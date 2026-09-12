using EcommerceServer.Dtos;

namespace EcommerceServer.Interfaces
{
    public interface IProducts
    {
        Task<PaginatedGetProductResponse<GetProducts>> GetProductsAsync(int page, int limit);
        Task<List<GetProducts>> GetProductsByCollectionAsync(string collection);
        Task<GetProductDetails> GetProductDetailsAsync(string slug);
        Task<bool> UploadProductAsync(UploadProduct uploadProduct);
        Task<bool>UpdateProductAsync( string id, UpdateProduct updateProduct);
        Task<bool> DeleteProductAsync(string id);

        
    }
}
