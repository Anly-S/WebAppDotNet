using Microsoft.AspNetCore.Mvc;
using WebApplicationDotNET.Services;
using WebApplicationDotNET.Implementations;
using WebApplicationDotNET.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                    response.status = "success";
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
            var response = new ApiResponse();
            var updated = _productService.UpdateProduct(product);
            if (!updated)
            {
                response.status = "fail";
                response.count = 0;
                response.error = "Product not found";
                return NotFound(response);

            }

            else {
                response.status = "success";
                response.data = product;
                response.count = 1;
                return Ok(response);

            }


        }

        [HttpDelete("delete/{productCode}", Name = "DeleteProduct")]
        public IActionResult DeleteProduct(string productCode)
        {
            var response = new ApiResponse();
            var deleted = _productService.DeleteProduct(productCode);
            if (!deleted)
            {

                response.status = "fail";
                response.count = 0;
                response.error = "Product not found";
                return NotFound(response);
            }

            else
            {
                response.status = "success";
                response.count = 0;
                return Ok(response);

            }
        }

    }
}


