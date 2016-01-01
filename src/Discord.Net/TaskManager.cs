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
        private readonly object _lock;
        private readonly Func<Task> _stopAction;

        private CancellationTokenSource _cancelSource;
        private Task _task;

        public bool WasStopExpected => _wasStopExpected;
        private bool _wasStopExpected;

        public Exception Exception => _stopReason?.SourceException;
        private ExceptionDispatchInfo _stopReason;

        internal TaskManager()
        {
            _lock = new object();
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

                lock (_lock)
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
                            SignalError(firstTask.Exception);
                        else
                            SignalStop();

                        //Wait for the other tasks, and signal their errors too just in case
                        try { await allTasks.ConfigureAwait(false); }
                        catch (AggregateException ex) { SignalError(ex.InnerExceptions.First()); } 
                        catch (Exception ex) { SignalError(ex); }

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

        public void SignalStop(bool isExpected = false)
        {
            lock (_lock)
            {
                if (isExpected)
                    _wasStopExpected = true;

                if (_task == null) return; //Are we running?
                if (_cancelSource.IsCancellationRequested) return;

                if (_cancelSource != null)
                    _cancelSource.Cancel();
            }
        }
        public Task Stop(bool isExpected = false)
        {
            Task task;
            lock (_lock)
            {
                if (isExpected)
                    _wasStopExpected = true;

                //Cache the task so we still have something to await if Cleanup is run really quickly
                task = _task;
                if (task == null) return TaskHelper.CompletedTask; //Are we running?
                if (_cancelSource.IsCancellationRequested) return task;

                if (_cancelSource != null)
                    _cancelSource.Cancel();
            }
            return task;
        }

        public void SignalError(Exception ex)
        {
            lock (_lock)
            {
                if (_stopReason != null) return;

                _stopReason = ExceptionDispatchInfo.Capture(ex);
                if (_cancelSource != null)
                    _cancelSource.Cancel();
            }
        }
        public Task Error(Exception ex)
        {
            Task task;
            lock (_lock)
            {
                if (_stopReason != null) return TaskHelper.CompletedTask;

                //Cache the task so we still have something to await if Cleanup is run really quickly
                task = _task ?? TaskHelper.CompletedTask;
                if (_cancelSource.IsCancellationRequested) return task;

                _stopReason = ExceptionDispatchInfo.Capture(ex);
                if (_cancelSource != null)
                    _cancelSource.Cancel();
            }
            return task;
        }

        /// <summary> Throws an exception if one was captured. </summary>
        public void ThrowException()
        {
            lock (_lock)
                _stopReason?.Throw();
        }
        public void ClearException()
        {
            lock (_lock)
            {
                _stopReason = null;
                _wasStopExpected = false;
            }
        }
    }
}
