using Dapr.Actors;

namespace FrontEnd.ActorDefine
{
    public interface IOrderStatusActor : IActor
    {
        Task<string> Paid(string orderId);

        Task<string> GetStatus(string orderId);

        Task StopTimerAsync(string name);
    }
}
