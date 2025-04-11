using Quartz;
using Quartz.Spi;

namespace ECommerce.Api.Services
{
    public class ScopedJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ScopedJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_serviceProvider.GetRequiredService(bundle.JobDetail.JobType);
        }
        public void ReturnJob(IJob job) { }
    }
}