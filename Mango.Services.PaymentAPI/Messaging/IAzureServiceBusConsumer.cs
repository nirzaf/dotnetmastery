using System.Threading.Tasks;

namespace Mango.Services.PaymentAPI.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}