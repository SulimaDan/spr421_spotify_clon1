using Quartz;

namespace spr421_spotify_clone.Jobs
{
    public class LogsCleaningJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var root = Directory.GetCurrentDirectory();
                var logs = Path.Combine(root, "logs");

                var files = Directory.GetFiles(logs);

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    var creationTime = fileInfo.CreationTime;
                    if (DateTime.Now.AddMinutes(-5) > creationTime)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
