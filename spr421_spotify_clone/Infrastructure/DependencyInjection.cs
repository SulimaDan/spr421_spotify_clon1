using Quartz;

namespace spr421_spotify_clone.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddJobs(this IServiceCollection services, params (Type type, string schedule)[] jobs)
        {
            services.AddQuartz(q =>
            {
                foreach (var job in jobs)
                {
                    var jobKey = new JobKey(job.type.Name);
                    q.AddJob(job.type, jobKey);
                    q.AddTrigger(opt => opt
                        .ForJob(jobKey)
                        .WithIdentity($"{jobKey}-trigger")
                        .WithCronSchedule(job.schedule));
                }
            });
        }
    }
}
