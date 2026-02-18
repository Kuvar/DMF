namespace DMF.Services.Interfaces
{
    public interface ICarService
    {
        Task<List<CarModel>> GetCarsAsync();
        Task<List<CarModel>> GetFavoriteCarsAsync(int userId);
        Task<ApiResponse<IEnumerable<CarModel>>> GetAllCarsAsync();

        Task<ApiResponse<CarModel>> GetCarByIdAsync(int id);

        Task<ApiResponse<PagedResponse<CarFilterResult>>> GetFilteredCarsAsync(CarFilterModel f);
    }
}
