using Quartz;

namespace spr421_spotify_clone.Jobs
{
    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("Hello quartz");
        }
    }
}
