using ECommerce.Application.Interfaces;
using Quartz;

namespace ECommerce.Api.Jobs
{
    public class MyJob : IJob
    {
        private readonly IOrderService _orderService;
        public MyJob(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _orderService.UpdateOrderStatusesAsync();
            return Task.CompletedTask;
        }
    }
}
