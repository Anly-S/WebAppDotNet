
using WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Services
{
    public interface ISalesService
    {
         IEnumerable<SalesDetails> GetAllSales();
        bool DeleteSale(int id);
        void RecordSales(SalesDetails sale);
    }
}
