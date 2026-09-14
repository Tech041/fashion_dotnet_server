using System.Text.Json;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EcommerceServer.Data;
using EcommerceServer.Dtos;
using EcommerceServer.Entities;
using EcommerceServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace EcommerceServer.Services
{
    public class Products(AppDbContext context, Cloudinary cloudinary) : IProducts
    {
       

        public async Task<GetProductDetails> GetProductDetailsAsync(string slug)
        {
            var product = await context.Products
                .Where(p => p.ProductSlug == slug)
                .Select(p => new GetProductDetails
                {
                    _id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    collection = p.Collection,
                    sizes = JsonSerializer.Deserialize<List<string>>(p.Sizes)!,
                    image = p.ImageUrl,
                    publicId = p.PublicId,
                    slug = p.ProductSlug
                })
                .FirstOrDefaultAsync();

            if (product == null) return null;
            

            return product;
        }


        public async Task<PaginatedGetProductResponse<GetProducts>> GetProductsAsync(int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var total = await context.Products.CountAsync();
            var pages = (int)Math.Ceiling(total / (double)limit);

            var products = await context.Products
                .OrderBy(p => p.Name)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => new GetProducts
                {
                    _id = p.Id,
                    name = p.Name,
                    slug = p.ProductSlug,
                    description = p.Description,
                    price = p.Price,
                    sizes = JsonSerializer.Deserialize<List<string>>(p.Sizes)!,
                    collection = p.Collection,
                    image = p.ImageUrl,
                    publicId = p.PublicId
                })
                .ToListAsync();

            return new PaginatedGetProductResponse<GetProducts>
            {
                success = true,
                products = products,
                total = total,
                page = page,
                pages = pages
            };
        }


        public async Task<List<GetProducts>> GetProductsByCollectionAsync(string collection)
        {
            var products = await context.Products
                .Where(p => p.Collection == collection)
                .OrderBy(p => p.Name)
                .Select(p => new GetProducts
                {
                    _id = p.Id,
                    name = p.Name,
                    slug = p.ProductSlug,
                    description = p.Description,
                    price = p.Price,
                    sizes = JsonSerializer.Deserialize<List<string>>(p.Sizes)!,
                    collection = p.Collection,
                    image = p.ImageUrl,
                    publicId = p.PublicId
                })
                .ToListAsync();

            return products;
        }


     
        public async Task<bool> UploadProductAsync(UploadProduct uploadProduct)
        {

            if (uploadProduct == null)
                throw new ArgumentNullException(nameof(uploadProduct), "UploadProduct payload is null");

            if (uploadProduct.Image == null)
                throw new Exception("No image file provided");

            if (cloudinary == null)
                throw new Exception("Cloudinary client not initialized. Check environment variables.");

            if (context == null)
                throw new Exception("Database context not injected. Check DI setup.");
            // 1. Compress image
            byte[] compressedBytes;
            using (var image = await Image.LoadAsync(uploadProduct.Image.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                     Size = new SixLabors.ImageSharp.Size(800, 800)
                }));

                using var ms = new MemoryStream();
                await image.SaveAsWebpAsync(ms, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder
                {
                    Quality = 60,          // lower quality → smaller size
                    FileFormat = WebpFileFormatType.Lossy // force lossy compression5
                });
                compressedBytes = ms.ToArray();
            }

            // 2. Upload to Cloudinary
            using var uploadStream = new MemoryStream(compressedBytes);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(uploadProduct.Image.FileName, uploadStream),
                Folder = "dotnetproducts"
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult?.SecureUrl == null)
                throw new Exception("Cloudinary upload failed");

            // 3. Save product in DB
            var product = new Product
            {
                
                Name = uploadProduct.Name,
                Price = uploadProduct.Price,
                Collection = uploadProduct.Collection,
                Description = uploadProduct.Description,
                Sizes = JsonSerializer.Serialize(uploadProduct.Sizes),
                ProductSlug = uploadProduct.ProductSlug,
                ImageUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateProductAsync(Guid id, UpdateProduct updateProduct)
        {
            var existingProduct = await context.Products.FindAsync(id);
            if (existingProduct == null) return false;

            existingProduct.Name = updateProduct.name;
            existingProduct.Price = updateProduct.price;
            existingProduct.Description = updateProduct.description;
            existingProduct.Collection = updateProduct.collection;
            await context.SaveChangesAsync();
            return true;
        }


       public async Task<bool> DeleteProductAsync(Guid id)
        {
            var productToDelete = await context.Products.FindAsync(id);
            var deletionParams = new DeletionParams(productToDelete.PublicId);
            if (productToDelete == null) return false;
            context.Products.Remove(productToDelete);
            await cloudinary.DestroyAsync(deletionParams);
            await context.SaveChangesAsync();
            return true;


        }
    }
}
