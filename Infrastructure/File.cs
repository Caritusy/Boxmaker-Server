namespace BoxMaker_Server
{
    public class File
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1); // 控制对文件的访问

        public static async Task<string> ReadAllTextAsync(string filePath)
        {
            await semaphore.WaitAsync(); // 请求许可
            try
            {
                // 执行文件读取操作
                string content = await System.IO.File.ReadAllTextAsync(filePath);
                return content;
            }
            finally
            {
                semaphore.Release(); // 释放许可
            }
        }

        public static async Task WriteAllTextAsync(string filePath, string content)
        {
            await semaphore.WaitAsync(); // 请求许可
            try
            {
                // 执行文件写入操作
                await System.IO.File.WriteAllTextAsync(filePath, content);
            }
            finally
            {
                semaphore.Release(); // 释放许可
            }
        }
        
    }

}
