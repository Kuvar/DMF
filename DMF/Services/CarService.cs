using System.Text.Json;

namespace DMF.Services
{
    public class CarService : ICarService
    {
        private readonly IApiService _apiService;

        public CarService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<CarModel>> GetCarsAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("cars.json");
            using var reader = new StreamReader(stream);

            var json = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<List<CarModel>>(json)
                   ?? new List<CarModel>();
        }

        public async Task<List<CarModel>> GetFavoriteCarsAsync(int userId)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("cars.json");
            using var reader = new StreamReader(stream);

            var json = await reader.ReadToEndAsync();

            var allCars = JsonSerializer.Deserialize<List<CarModel>>(json)
                          ?? new List<CarModel>();

            return allCars.Take(5).ToList();
        }

        // --------------------------------------------------
        // GET ALL
        // --------------------------------------------------
        public async Task<ApiResponse<IEnumerable<CarModel>>> GetAllCarsAsync()
        {
            return await _apiService
                .GetAsync<IEnumerable<CarModel>>("cars");
        }

        // --------------------------------------------------
        // GET BY ID
        // --------------------------------------------------
        public async Task<ApiResponse<CarModel>> GetCarByIdAsync(int id)
        {
            return await _apiService
                .GetAsync<CarModel>($"cars/{id}");
        }

        // --------------------------------------------------
        // FILTER
        // --------------------------------------------------
        public async Task<ApiResponse<PagedResponse<CarFilterResult>>> GetFilteredCarsAsync(CarFilterModel f)
        {
            var query = new List<string>();

            void Add(string key, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    query.Add($"{key}={Uri.EscapeDataString(value)}");
            }

            void AddInt(string key, int value)
            {
                if (value != 0)
                    query.Add($"{key}={value}");
            }

            Add("brand", f.Brand);
            Add("model", f.Model);
            Add("fuel", f.Fuel);
            Add("transmission", f.Transmission);

            AddInt("owners", f.Owners);
            AddInt("priceMore", f.PriceMore);
            AddInt("priceLess", f.PriceLess);
            AddInt("drivenMore", f.DrivenMore);
            AddInt("drivenLess", f.DrivenLess);
            AddInt("age", f.Age);
            AddInt("dealersID", f.DealersID);

            // always include these
            query.Add($"isActive={f.IsActive}");
            query.Add($"page={f.Page}");
            query.Add($"pageSize={f.PageSize}");
            query.Add($"sortBy={f.SortBy}");
            query.Add($"sortDir={f.SortDir}");

            var endpoint = $"cars/filter";

            if (query.Count > 0)
                endpoint += "?" + string.Join("&", query);

            return await _apiService
                .GetAsync<PagedResponse<CarFilterResult>>(endpoint);
        }

    }
}
