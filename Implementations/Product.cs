
using WebApplicationDotNET.Services;

namespace WebApplicationDotNET.Implementations
{
    public class ProductService : IProductService
    {
        public class ProductDetails
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        private readonly string productsFilePath = "C:\\Users\\anly.s\\source\\repos\\ProductStoreApp\\ProductStoreApp\\products.csv";
        private readonly ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
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

        public bool UpdateProduct(ProductDetails product)
        {
            var products = ReadProductsFromCsv(productsFilePath).ToList();
            var existingProduct = products.FirstOrDefault(p => p.ProductCode == product.ProductCode);

            if (existingProduct == null)
            {
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
