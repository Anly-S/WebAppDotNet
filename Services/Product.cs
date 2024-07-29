using WebApplicationDotNET.Implementations;
using static WebApplicationDotNET.Implementations.ProductService;

namespace WebApplicationDotNET.Services
{
    public interface IProductService
    {
        IEnumerable<ProductDetails> GetAllProducts();
        void AddProduct(ProductDetails product);
        bool UpdateProduct(ProductDetails product);
        bool DeleteProduct(string productCode);
    }
}

