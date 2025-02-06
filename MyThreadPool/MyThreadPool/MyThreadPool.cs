// <copyright file="Program.cs" company="NematMusaev">
// Copyright (c) Nemat Musaev. All rights reserved.
// </copyright>
namespace MyThreadPool;

/// <summary>
/// Thread pool class.
/// </summary>
public class ThreadPool
{
    private readonly object lockObject = new ();
    private readonly CancellationTokenSource cancellationToken = new ();
    private readonly Thread[] threads;
    private readonly Queue<Action> queue = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">count of threds.</param>
    public ThreadPool(int threadCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);

        threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(WorkerLoop);
            threads[i].Start();
        }
    }

    private void WorkerLoop()
    {
        while (!cancellationToken.IsCancellationRequested || queue.Count > 0)
        {
            Action? work = null;
            lock (lockObject)
            {
                while (queue.Count == 0 && !cancellationToken.IsCancellationRequested)
                {
                    Monitor.Wait(lockObject);
                }

                if (queue.Count > 0)
                {
                    work = queue.Dequeue();
                }
            }
            work?.Invoke();
        }
    }
    
    /// <summary>
    /// interrupts working.
    /// </summary>
    public void ShutDown()
    {
        lock (lockObject)
        {
            cancellationToken.Cancel();
            Monitor.PulseAll(lockObject);
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    /// <summary>
    /// Gets my Task and added him to action queue.
    /// </summary>
    /// <typeparam name="TResult">MyTask</typeparam>
    /// <param name="func">function of our task.</param>
    /// <returns>MyTask object</returns>
    /// <exception cref="OperationCanceledException"Cansel Exception.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        lock (lockObject)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            var myTask = new MyTask<TResult>(func, this);
            queue.Enqueue(myTask.Execute);
            Monitor.Pulse(lockObject);

            return myTask;
        }
    }
    
    /// <summary>
    /// class MyTask
    /// </summary>
    /// <typeparam name="TResult">general type.</typeparam>
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly CancellationToken cancellationToken;
        private readonly ThreadPool pool;
        private readonly object objectLock = new ();
        private readonly List<Action> continuations = new ();
        private Exception? taskException;
        private TResult? taskResult;
        private Func<TResult>? function;
        private bool isFinished;

        /// <summary>
        /// Gets Result.
        /// </summary>
        public TResult Result
        {
            get
            {
                lock (objectLock)
                {
                    while (!IsFinished)
                    {
                        Monitor.Wait(objectLock);
                    }

                    if (taskException != null)
                    {
                        throw new AggregateException(taskException);
                    }

                    return taskResult ?? throw new InvalidOperationException("Task completed without a result.");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask"/> class.
        /// </summary>
        /// <param name="func">task function.</param>
        /// <param name="threadPool">pool (array) of threads.</param>
        public MyTask(Func<TResult> func, ThreadPool threadPool)
        {
            pool = threadPool;
            function = func;
            cancellationToken = threadPool.cancellationToken.Token;
        }

        public bool IsFinished
        {
            get { lock (objectLock) { return isFinished; } }
            private set { lock (objectLock) { isFinished = value; } }
        }

        public void Execute()
        {
            try
            {
                if (function != null)
                {
                    taskResult = function();
                    function = null;
                }
            }
            catch (Exception ex)
            {
                taskException = ex;
            }

            lock (objectLock)
            {
                IsFinished = true;
                Monitor.PulseAll(objectLock);
                foreach (var continuation in continuations)
                {
                    lock (objectLock)
                    {
                        pool.queue.Enqueue(continuation);
                        Monitor.Pulse(objectLock);
                    }
                }

                continuations.Clear();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            lock (objectLock)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                if (IsFinished)
                {
                    return pool.Submit(() => continuation(Result));
                }

                var nextTask = new MyTask<TNewResult>(() => continuation(Result), pool);
                continuations.Add(nextTask.Execute);
                return nextTask;
            }
        }
    }
}