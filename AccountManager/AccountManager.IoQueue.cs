using System.Collections.Concurrent;

namespace BoxMaker_Server
{
    public partial class AccountManager
    {
        private sealed class QueuedIoWork
        {
            public string Name { get; init; } = "";
            public Action Work { get; init; } = () => { };
        }

        private static readonly BlockingCollection<QueuedIoWork> IoQueue = new BlockingCollection<QueuedIoWork>(new ConcurrentQueue<QueuedIoWork>());
        private static readonly Thread IoQueueThread = StartIoQueueWorker();

        private static Thread StartIoQueueWorker()
        {
            Thread thread = new Thread(ProcessIoQueue)
            {
                IsBackground = true,
                Name = "Boxmaker IO Queue",
            };
            thread.Start();
            return thread;
        }

        private static void EnqueueIo(string name, Action work)
        {
            try
            {
                IoQueue.Add(new QueuedIoWork
                {
                    Name = name,
                    Work = work,
                });
            }
            catch (InvalidOperationException)
            {
                // If the queue is no longer accepting work during shutdown, run inline
                // so the caller's state change is not silently lost.
                work();
            }
        }

        private static void ProcessIoQueue()
        {
            foreach (QueuedIoWork work in IoQueue.GetConsumingEnumerable())
            {
                try
                {
                    work.Work();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"后台IO任务失败 [{work.Name}]: {ex}");
                }
            }
        }
    }
}
