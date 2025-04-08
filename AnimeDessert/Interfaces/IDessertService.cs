using AnimeDessert.Models;

namespace AnimeDessert.Interfaces
{
    public interface IDessertService
    {
        // base CRUD
        Task<IEnumerable<DessertDto>> ListDesserts();
        Task<DessertDto?> FindDessert(int id);
        Task<ServiceResponse> UpdateDessert(DessertDto dessertDto);
        Task<ServiceResponse> AddDessert(DessertDto dessertDto);
        Task<ServiceResponse> DeleteDessert(int id);

        // related methods
        Task<IEnumerable<DessertDto>> ListDessertsForIngredient(int id);
        Task<ServiceResponse> LinkDessertToIngredient(int dessertId, int ingredientId);
        Task<ServiceResponse> UnlinkDessertFromIngredient(int dessertId, int ingredientId);

        Task<IEnumerable<ImageDto>> ListImagesForDessert(int id);

        Task<ServiceResponse> AddImagesToDessert(int id, AddImagesToDessertRequest request);

        Task<ServiceResponse> RemoveImagesFromDessert(int id, RemoveImagesFromDessertRequest request);
        

    }
}
