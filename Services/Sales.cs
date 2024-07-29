
using static WebApplicationDotNET.Implementations.Sales;

namespace WebApplicationDotNET.Services
{
    public interface ISalesService
    {
         IEnumerable<SalesDetails> GetAllSales();
        bool DeleteSale(int id);
    }
}
