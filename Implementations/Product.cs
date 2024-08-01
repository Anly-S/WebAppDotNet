using WebApplicationDotNET.Services;
using WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Implementations
{
    public class ProductService : IProductService
    {
        private readonly string productsFilePath = "C:\\Users\\anly.s\\source\\repos\\WebApplicationDotNET\\WebApplicationDotNET\\DataFiles\\products.csv";
        private readonly ILogger<ProductService> _logger;
        private readonly ISalesService _salesService;

        public ProductService(ILogger<ProductService> logger, ISalesService salesService)
        {
            _logger = logger;
            _salesService = salesService;
        }

        private IEnumerable<ProductDetails> ReadProductsFromCsv(string filePath)
        {
            var products = new List<ProductDetails>();

            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var values = line.Split(',');

                    if (values.Length != 4)
                    {
                        continue;
                    }

                    if (decimal.TryParse(values[2], out decimal price) &&
                        int.TryParse(values[3], out int stock))
                    {
                        var product = new ProductDetails
                        {
                            ProductCode = values[0],
                            ProductName = values[1],
                            Price = price,
                            Stock = stock
                        };
                        products.Add(product);
                    }
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error reading the products CSV file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the products CSV file.");
            }

            return products;
        }

        private void WriteProductsToCsv(string filePath, IEnumerable<ProductDetails> products)
        {
            try
            {
                var lines = products.Select(p => $"{p.ProductCode},{p.ProductName},{p.Price},{p.Stock}");
                File.WriteAllLines(filePath, lines);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error writing to the products CSV file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while writing to the products CSV file.");
            }
        }

        public void AddProduct(ProductDetails product)
        {
            var products = ReadProductsFromCsv(productsFilePath).ToList();
            var existingProduct = products.FirstOrDefault(p => p.ProductCode == product.ProductCode);

            if (existingProduct != null)
            {
                existingProduct.Stock += product.Stock;
            }
            else
            {
                products.Add(product);
            }

            WriteProductsToCsv(productsFilePath, products);
        }

        public ProductDetails BuyProduct(string productCode, int quantity)
        {
            var products = ReadProductsFromCsv(productsFilePath).ToList();
            var foundProduct = products.FirstOrDefault(p => p.ProductCode == productCode);

            if (foundProduct != null)
            {
                if (foundProduct.Stock >= quantity)
                {
                    foundProduct.Stock -= quantity;
                    var sale = new SalesDetails
                    {
                        Id = _salesService.GetAllSales().Count() + 1,
                        Timestamp = DateTime.Now,
                        ProductCode = foundProduct.ProductCode,
                        ProductName = foundProduct.ProductName,
                        Quantity = quantity,
                        Price = foundProduct.Price * quantity
                    };

                    _salesService.RecordSales(sale);
                    WriteProductsToCsv(productsFilePath, products);
                    return foundProduct;
                }
                else
                {
                    _logger.LogWarning("Insufficient stock for product: {ProductCode}", productCode);
                    return null;
                }
            }

            _logger.LogWarning("Product not found: {ProductCode}", productCode);
            return null;
        }

        public bool UpdateProduct(ProductDetails product)
        {
            var products = ReadProductsFromCsv(productsFilePath).ToList();
            var existingProduct = products.FirstOrDefault(p => p.ProductCode == product.ProductCode);

            if (existingProduct == null)
            {
                _logger.LogWarning("Product not found: {ProductCode}", product.ProductCode);
                return false;
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;

            WriteProductsToCsv(productsFilePath, products);
            return true;
        }

        public bool DeleteProduct(string productCode)
        {
            var products = ReadProductsFromCsv(productsFilePath).ToList();
            var productToRemove = products.FirstOrDefault(p => p.ProductCode == productCode);

            if (productToRemove == null)
            {
                _logger.LogWarning("Product not found: {ProductCode}", productCode);
                return false;
            }

            products.Remove(productToRemove);
            WriteProductsToCsv(productsFilePath, products);
            return true;
        }

        public IEnumerable<ProductDetails> GetAllProducts()
        {
            return ReadProductsFromCsv(productsFilePath);
        }
    }
}
