using  WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Services
{
    public interface IProductService
    {
        IEnumerable<ProductDetails> GetAllProducts();
        void AddProduct(ProductDetails product);
        ProductDetails BuyProduct(string productCode,int quantity);
        bool UpdateProduct(ProductDetails product);
        bool DeleteProduct(string productCode);
    }
}

