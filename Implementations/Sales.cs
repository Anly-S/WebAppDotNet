
using System.Globalization;
using WebApplicationDotNET.Services;
using WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Implementations
{
    public class Sales : ISalesService
    {

        private readonly string salesFilePath = "C:\\Users\\anly.s\\source\\repos\\ProductStoreApp\\ProductStoreApp\\sales.csv";
        private readonly ILogger<Sales> _logger;
        private List<SalesDetails> _sales;

        public Sales(ILogger<Sales> logger)
        {
            _logger = logger;
            _sales = ReadSalesFromCsv(salesFilePath).ToList();
        }

        private IEnumerable<SalesDetails> ReadSalesFromCsv(string filePath)
        {
            var sales = new List<SalesDetails>();

            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var values = line.Split(',');

                    if (values.Length != 6)
                    {
                        continue;
                    }

                    if (int.TryParse(values[0], out int id) &&
                        DateTime.TryParseExact(values[1], "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp) &&
                        int.TryParse(values[4], out int quantity) &&
                        decimal.TryParse(values[5], out decimal price))
                    {
                        var sale = new SalesDetails
                        {
                            Id = id,
                            Timestamp = timestamp,
                            ProductCode = values[2],
                            ProductName = values[3],
                            Quantity = quantity,
                            Price = price
                        };
                        sales.Add(sale);
                    }
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error reading the sales CSV file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the sales CSV file.");
            }

            return sales;
        }

        private void WriteSalesToCsv(string filePath, IEnumerable<SalesDetails> sales)
        {
            try
            {
                var lines = sales.Select(s => $"{s.Id},{s.Timestamp:dd-MM-yyyy HH:mm:ss},{s.ProductCode},{s.ProductName},{s.Quantity},{s.Price}");
                File.WriteAllLines(filePath, lines);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error writing to the sales CSV file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while writing to the sales CSV file.");
            }
        }

        public IEnumerable<SalesDetails> GetAllSales()
        {
            return _sales;
        }

        public bool DeleteSale(int id)
        {
            var sale = _sales.FirstOrDefault(s => s.Id == id);
            if (sale == null)
            {
                return false;
            }

            _sales.Remove(sale);
            WriteSalesToCsv(salesFilePath, _sales);
            return true;
        }

        public void RecordSales(SalesDetails sale)
        {
            _sales.Add(sale);
            WriteSalesToCsv(salesFilePath, _sales);
        }

    }
}
