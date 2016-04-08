using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Helper class used to manage several tasks and keep them in sync. If any single task errors or stops, all other tasks will also be stopped. </summary>
    public class TaskManager
    {
        private readonly AsyncLock _lock;
        private readonly Func<Task> _stopAction;
        private ExceptionDispatchInfo _stopReason;

        private CancellationTokenSource _cancelSource;
        private Task _task;

        public bool StopOnCompletion { get; }
        public bool WasStopExpected { get; private set; }
        public CancellationToken CancelToken => _cancelSource.Token;

        public Exception Exception => _stopReason?.SourceException;

        internal TaskManager(bool stopOnCompletion)
        {
            _lock = new AsyncLock();
            StopOnCompletion = stopOnCompletion;
        }
        public TaskManager(Action stopAction, bool stopOnCompletion = true)
            : this(stopOnCompletion)
        {
            _stopAction = TaskHelper.ToAsync(stopAction);
        }
        public TaskManager(Func<Task> stopAction, bool stopOnCompletion = true)
            : this(stopOnCompletion)
        {
            _stopAction = stopAction;
        }

        public async Task Start(IEnumerable<Task> tasks, CancellationTokenSource cancelSource)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));
            if (cancelSource == null) throw new ArgumentNullException(nameof(cancelSource));

            while (true)
            {
                var task = _task;
                if (task != null)
                    await Stop().ConfigureAwait(false);

                using (await _lock.LockAsync().ConfigureAwait(false))
                {
                    _cancelSource = cancelSource;

                    if (_task != null)
                        continue; //Another thread sneaked in and started this manager before we got a lock, loop and try again

                    _stopReason = null;
                    WasStopExpected = false;

                    Task[] tasksArray = tasks.ToArray();

                    _task = Task.Run(async () =>
                    {
                        if (tasksArray.Length > 0)
                        {
                            Task<Task> anyTask = tasksArray.Length > 0 ? Task.WhenAny(tasksArray) : null;
                            Task allTasks = tasksArray.Length > 0 ? Task.WhenAll(tasksArray) : null;
                            //Wait for the first task to stop or error
                            Task firstTask = await anyTask.ConfigureAwait(false);

                            //Signal the rest of the tasks to stop
                            if (firstTask.Exception != null)
                                await SignalError(firstTask.Exception).ConfigureAwait(false);
                            else if (StopOnCompletion) //Unless we allow for natural completions
                                await SignalStop().ConfigureAwait(false);

                            //Wait for the other tasks, and signal their errors too just in case
                            try { await allTasks.ConfigureAwait(false); }
                            catch (AggregateException ex) { await SignalError(ex.InnerExceptions.First()).ConfigureAwait(false); }
                            catch (Exception ex) { await SignalError(ex).ConfigureAwait(false); }
                        }

                        if (!StopOnCompletion && !_cancelSource.IsCancellationRequested)
                        {
                            try { await Task.Delay(-1, _cancelSource.Token).ConfigureAwait(false); } //Pause until TaskManager is stopped
                            catch (OperationCanceledException) { }
                        }

                        //Run the cleanup function within our lock
                        if (_stopAction != null)
                            await _stopAction().ConfigureAwait(false);
                        _task = null;
                        _cancelSource = null;
                    });
                    return;
                }
            }
        }

        public async Task SignalStop(bool isExpected = false)
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                if (isExpected)
                    WasStopExpected = true;

                Cancel();
            }
        }
        public async Task Stop(bool isExpected = false)
        {
            Task task;
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                if (isExpected)
                    WasStopExpected = true;

                //Cache the task so we still have something to await if Cleanup is run really quickly
                task = _task ?? TaskHelper.CompletedTask;
                Cancel();
            }
            await task.ConfigureAwait(false);
        }

        public async Task SignalError(Exception ex)
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                if (_stopReason != null) return;

                Cancel(ex);
            }
        }
        public async Task Error(Exception ex)
        {
            Task task;
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                if (_stopReason != null) return;

                //Cache the task so we still have something to await if Cleanup is run really quickly
                task = _task ?? TaskHelper.CompletedTask;
                Cancel(ex);
            }
            await task.ConfigureAwait(false);
        }
        private void Cancel(Exception ex = null)
        {
            var source = _cancelSource;
            if (source != null && !source.IsCancellationRequested)
            {
                if (ex != null)
                    _stopReason = ExceptionDispatchInfo.Capture(ex);
                _cancelSource.Cancel();
            }
        }

        /// <summary> Throws an exception if one was captured. </summary>
        public void ThrowException()
        {
            using (_lock.Lock())
            {
                if (!WasStopExpected)
                    _stopReason?.Throw();
            }
        }
        public void ClearException()
        {
            using (_lock.Lock())
            {
                _stopReason = null;
                WasStopExpected = false;
            }
        }
    }
}
