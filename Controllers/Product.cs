using Microsoft.AspNetCore.Mvc;
using WebApplicationDotNET.Services;
using WebApplicationDotNET.Implementations;
using WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products", Name = "GetProducts")]
        public IActionResult GetProducts()
        {
            var response = new ApiResponse();
            var products = _productService.GetAllProducts();
            if (products == null || !products.Any())
            {

                response.status = "fail";
                response.count = 0;
                response.error = "No products found";
                return BadRequest(response);               
            }

            else
            {
                response.status = "success";
                response.data = products;
                response.count = products.Count();
                response.error = (string)null;
                return Ok(response);
            };
        }

        [HttpPost("add", Name = "AddProduct")]
        public IActionResult AddProduct([FromBody] ProductService.ProductDetails product)
        {
            var response = new ApiResponse();
            try
            {
                _productService.AddProduct(product);
                {

                    response.status = "fail";
                    response.data = product;
                    response.count = 1;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.status = "fail";
                response.error = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("update", Name = "UpdateProduct")]
        public IActionResult UpdateProduct([FromBody] ProductService.ProductDetails product)
        {
            var updated = _productService.UpdateProduct(product);
            if (!updated)
            {
                return NotFound(new
                {
                    status = "fail",
                    data = (object)null,
                    count = 0,
                    error = "Product not found"
                });
            }

            return Ok(new
            {
                status = "success",
                data = product,
                count = 1,
                error = (string)null
            });
        }

        [HttpDelete("delete/{productCode}", Name = "DeleteProduct")]
        public IActionResult DeleteProduct(string productCode)
        {
            var deleted = _productService.DeleteProduct(productCode);
            if (!deleted)
            {
                return NotFound(new
                {
                    status = "fail",
                    data = (object)null,
                    count = 0,
                    error = "Product not found"
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


