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
    public sealed class TaskManager
    {
        private readonly AsyncLock _lock;
        private readonly Func<Task> _stopAction;

        private CancellationTokenSource _cancelSource;
        private Task _task;

        public bool WasStopExpected => _wasStopExpected;
        private bool _wasStopExpected;

        public Exception Exception => _stopReason?.SourceException;
        private ExceptionDispatchInfo _stopReason;

        internal TaskManager()
        {
            _lock = new AsyncLock();
        }
        public TaskManager(Action stopAction)
            : this()
        {
            _stopAction = TaskHelper.ToAsync(stopAction);
        }
        public TaskManager(Func<Task> stopAction)
            : this()
        {
            _stopAction = stopAction;
        }

        public async Task Start(IEnumerable<Task> tasks, CancellationTokenSource cancelSource)
        {
            while (true)
            {
                var task = _task;
                if (task != null)
                    await Stop().ConfigureAwait(false);

                using (await _lock.LockAsync())
                {
                    _cancelSource = cancelSource;

                    if (_task != null)
                        continue; //Another thread sneaked in and started this manager before we got a lock, loop and try again

                    _stopReason = null;
                    _wasStopExpected = false;

                    Task[] tasksArray = tasks.ToArray();
                    Task<Task> anyTask = Task.WhenAny(tasksArray);
                    Task allTasks = Task.WhenAll(tasksArray);

                    _task = Task.Run(async () =>
                    {
                        //Wait for the first task to stop or error
                        Task firstTask = await anyTask.ConfigureAwait(false);

                        //Signal the rest of the tasks to stop
                        if (firstTask.Exception != null)
                            await SignalError(firstTask.Exception);
                        else
                            await SignalStop();

                        //Wait for the other tasks, and signal their errors too just in case
                        try { await allTasks.ConfigureAwait(false); }
                        catch (AggregateException ex) { await SignalError(ex.InnerExceptions.First()); } 
                        catch (Exception ex) { await SignalError(ex); }

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
            using (await _lock.LockAsync())
            {
                if (isExpected)
                    _wasStopExpected = true;

                if (_task == null) return; //Are we running?
                if (_cancelSource.IsCancellationRequested) return;

                if (_cancelSource != null)
                    _cancelSource.Cancel();
            }
        }
        public async Task Stop(bool isExpected = false)
        {
            Task task;
            using (await _lock.LockAsync())
            {
                if (isExpected)
                    _wasStopExpected = true;

                //Cache the task so we still have something to await if Cleanup is run really quickly
                task = _task;
                if (task == null) return; //Are we running?

                if (!_cancelSource.IsCancellationRequested && _cancelSource != null)
                    _cancelSource.Cancel();
            }
            await task;
        }

        public async Task SignalError(Exception ex)
        {
            using (await _lock.LockAsync())
            {
                if (_stopReason != null) return;

                _stopReason = ExceptionDispatchInfo.Capture(ex);
                if (_cancelSource != null)
                    _cancelSource.Cancel();
            }
        }
        public async Task Error(Exception ex)
        {
            Task task;
            using (await _lock.LockAsync())
            {
                if (_stopReason != null) return;

                //Cache the task so we still have something to await if Cleanup is run really quickly
                task = _task ?? TaskHelper.CompletedTask;
                if (!_cancelSource.IsCancellationRequested)
                {
                    _stopReason = ExceptionDispatchInfo.Capture(ex);
                    if (_cancelSource != null)
                        _cancelSource.Cancel();
                }
            }
            await task;
        }

        /// <summary> Throws an exception if one was captured. </summary>
        public void ThrowException()
        {
            using (_lock.Lock())
                _stopReason?.Throw();
        }
        public void ClearException()
        {
            using (_lock.Lock())
            {
                _stopReason = null;
                _wasStopExpected = false;
            }
        }
    }
}
