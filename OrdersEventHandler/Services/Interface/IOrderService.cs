using OrdersEventHandler.Models;

namespace OrdersEventHandler.Services
{
    public interface IOrderService
    {
        void ProceedOrderNotification(OrderNotification order);
    }
}
