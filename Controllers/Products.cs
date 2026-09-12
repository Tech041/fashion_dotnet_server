using EcommerceServer.Dtos;
using EcommerceServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/product")]
[ApiController]
public class ProductController(IProducts productService) : ControllerBase
{
   
    [HttpPost("upload")]
    public async Task<ActionResult> UploadProduct([FromForm] UploadProduct uploadProduct)
    {
        try
        {
            var result = await productService.UploadProductAsync(uploadProduct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // handle errors gracefully
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("all-products")]

    public async Task<ActionResult<PaginatedGetProductResponse<GetProducts>>> GetProducts(
    [FromQuery] int page = 1,
    [FromQuery] int limit = 10)
    {
        try
        {
            var result = await productService.GetProductsAsync(page, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // log error here with ILogger
            return StatusCode(500, new PaginatedGetProductResponse<GetProducts>
            {
                success = false,
                products = new List<GetProducts>(),
                total = 0,
                page = page,
                pages = 0
            });
        }
    }

    [HttpGet("collection")]
    public async Task<ActionResult<List<GetProducts>>> GetProductsByCollection([FromQuery] string collection)
    {
        try
        {
            var products = await productService.GetProductsByCollectionAsync(collection);
            return Ok(new
            {
                success = true,
                products = products
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new List<GetProducts>());
        }
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<GetProductDetails>> GetProductBySlug([FromRoute] string slug)
    {
        try
        {
            var product = await productService.GetProductDetailsAsync(slug);

            if (product == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No product found with slug '{slug}'"
                });
            }

            return Ok(new
            {
                success = true,
                product
            });
        }
        catch (Exception ex)
        {
            // Log ex if needed
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while fetching product details."
            });
        }
    }

     
    [HttpPatch("update/{id}")]
    public async Task<ActionResult> UpdateProduct([FromRoute] string id, [FromBody] UpdateProduct updateProduct)
    {
        try
        {
            var updated = await productService.UpdateProductAsync(id, updateProduct);

            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No product found with id '{id}'"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Product updated successfully"
            });
        }
        catch (Exception ex)
        {
            // log ex if needed
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating the product"
            });
        }
    }
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteProductAsync([FromRoute] string id)
    {
        try
        {
            var deleted = await productService.DeleteProductAsync(id);

            if (!deleted)
                return NotFound(new { success = false, message = "Product not found" });

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log ex if needed
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while deleting product"
            });
        }
    }


}
