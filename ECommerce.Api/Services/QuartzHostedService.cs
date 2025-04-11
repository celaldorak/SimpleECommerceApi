using ECommerce.Api.Jobs;
using Quartz;
using Quartz.Impl;

namespace ECommerce.Api.Services
{
    public class QuartzHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public QuartzHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<MyJob>()
                .WithIdentity("myJob", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(60)
                .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}