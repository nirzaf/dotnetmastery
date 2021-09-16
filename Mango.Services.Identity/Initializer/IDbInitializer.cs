using System.Threading.Tasks;

namespace Mango.Services.Identity.Initializer
{
    public interface IDbInitializer
    {
        public Task Initialize();
    }
}