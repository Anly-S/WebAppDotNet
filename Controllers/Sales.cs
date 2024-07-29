using Microsoft.AspNetCore.Mvc;
using WebApplicationDotNET.Services;

namespace WebApplicationDotNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        public class Sale
        {
            public int Id { get; set; }
            public DateTime Timestamp { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ILogger<SalesController> logger, ISalesService salesService)
        {
            _salesService = salesService;
            _logger = logger;
        }

        [HttpGet("sales", Name = "GetSales")]
        public IActionResult GetSales()
        {
            var sales = _salesService.GetAllSales();
            if (sales == null || !sales.Any())
            {
                return Ok(new
                {
                    status = "fail",
                    data = (object)null,
                    count = 0,
                    error = "No sales found"
                });
            }

            return Ok(new
            {
                status = "success",
                data = sales,
                count = sales.Count(),
                error = (string)null
            });
        }

        [HttpDelete("delete/{id}", Name = "DeleteSale")]
        public IActionResult DeleteSale(int id)
        {
            var deleted = _salesService.DeleteSale(id);
            if (!deleted)
            {
                return NotFound(new
                {
                    status = "fail",
                    data = (object)null,
                    count = 0,
                    error = "Sale not found"
                });
            }

            return Ok(new
            {
                status = "success",
                data = (object)null,
                count = 0,
                error = (string)null
            });
        }
    }
}
