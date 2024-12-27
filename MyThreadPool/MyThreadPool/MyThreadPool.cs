namespace MyThreadPoolNamespace;

/// <summary>
/// Thread pool class.
/// </summary>
public class MyThreadPool
{
    private readonly object lockObject = new();
    private readonly CancellationTokenSource canselToken = new();
    private readonly Thread[] threads;
    private readonly Queue<Action> queue = new();
    private readonly AutoResetEvent workAvailable = new(false);
    private readonly ManualResetEvent terminationEvent = new(false);

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">count of threds.</param>
    public MyThreadPool(int threadCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(threadCount);

        threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(() => WorkerLoop());
            threads[i].Start();
        }
    }

    private void WorkerLoop()
    {
        while (!canselToken.IsCancellationRequested || queue.Count > 0)
        {
            Action? work = null;
            lock (lockObject)
            {
                while (queue.Count == 0 && !canselToken.IsCancellationRequested)
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
            canselToken.Cancel();
            terminationEvent.Set();
            Monitor.PulseAll(lockObject);
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    /// <summary>
    /// GEts MyTask.
    /// </summary>
    /// <typeparam name="TResult">MyTask</typeparam>
    /// <param name="func">function of our task.</param>
    /// <returns>MyTask object</returns>
    /// <exception cref="OperationCanceledException"Cansel Exception.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        lock (lockObject)
        {
            if (canselToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            var myTask = new MyTask<TResult>(func, this);

            lock (lockObject)
            {
                queue.Enqueue(myTask.Execute);
                Monitor.Pulse(lockObject);
            }
            workAvailable.Set();
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
        private readonly MyThreadPool pool;
        private readonly object objectLock = new();
        private readonly List<Action> continuations = new();
        private Exception? taskException;
        private TResult? taskResult;
        private Func<TResult>? function;
        private bool isFinished;
        /// <summary>
        /// Gets Result.
        /// </summary>
        public TResult Result => GetResult();

        /// <summary>
        /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
        /// </summary>
        /// <param name="func">task function</param>
        /// <param name="threadPool"></param>
        public MyTask(Func<TResult> func, MyThreadPool threadPool)
        {
            pool = threadPool;
            function = func;pool
            cancellationToken = threadPool.canselToken.Token;
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

                    pool.workAvailable.Set();
                }
                continuations.Clear();
            }
        }

        private TResult GetResult()
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

/// <summary>
/// MyTask interface.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IMyTask<out TResult>
{
    bool IsFinished { get; }
    TResult Result { get; }
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}
