namespace MyThreadPoolNamespace;

public class MyThreadPool
{
    private readonly object lockObject = new();
    private readonly CancellationTokenSource canselToken = new();
    private readonly Thread[] threads;
    private readonly Queue<Action> queue = new();
    private readonly AutoResetEvent workAvailable = new(false);
    private readonly ManualResetEvent terminationEvent = new(false);

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
        public TResult Result => GetResult();

        public MyTask(Func<TResult> func, MyThreadPool threadPool)
        {
            pool = threadPool;
            function = func;
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

public interface IMyTask<out TResult>
{
    bool IsFinished { get; }
    TResult Result { get; }
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}
