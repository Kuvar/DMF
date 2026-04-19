using DMF.Helpers;
using System.Text.Json;

namespace DMF.Services
{
    public class CarService : ICarService
    {
        private readonly IApiService _apiService;
        private readonly IBlobService _blobService;

        public CarService(IApiService apiService, IBlobService blobService)
        {
            _apiService = apiService;
            _blobService = blobService;
        }

        public async Task<List<CarModel>> GetCarsAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("cars.json");
            using var reader = new StreamReader(stream);

            var json = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<List<CarModel>>(json)
                   ?? new List<CarModel>();
        }

        public async Task<ApiResponse<IEnumerable<CarFilterResult>>> GetFavoriteCarsAsync(int userId)
        {
            return await _apiService.GetAsync<IEnumerable<CarFilterResult>>($"wishlist/{userId}");
        }

        public async Task<ApiResponse<bool>> ToggleWishlistAsync(int userId, int carId)
        {
            try
            {
                var endpoint = $"wishlist/toggle?userDetailId={userId}&carDetailId={carId}";

                return await _apiService.PostAsync<object, bool>(endpoint, new { });
            }
            catch (Exception ex)
            {
                throw;
            }
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
            AddInt("userDetailID", f.UserDetailID);
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


        public async Task<ApiResponse<bool>> AddCarAsync(AddCarModel model, IEnumerable<ImageItem> images, string dealerName, int dealerId, Func<double, Task>? progressCallback = null)
        {
            if (images == null || !images.Any())
                throw new Exception("At least one image is required.");

            if (images.Count() > 20)
                throw new Exception("Maximum 20 images allowed.");

            // --------------------------------------
            // STEP 1: Create Car (without images)
            // --------------------------------------
            var createResponse = await _apiService
                .PostAsync<AddCarModel, int>("cars", model);

            if (!createResponse.Success || createResponse.Data == 0)
                throw new Exception("Car creation failed.");

            var carId = createResponse.Data;

            // --------------------------------------
            // STEP 2: Upload Images to Blob
            // --------------------------------------
            var uploadedUrls = new List<string>();
            var uploadedBlobs = new List<string>();

            var safeDealerName = dealerName
                .Replace(" ", "_")
                .ToLower();

            var dealerFolder = $"{safeDealerName}_{dealerId}";
            var carFolder = $"{carId}";

            int total = images.Count();
            int completed = 0;

            try
            {
                var tasks = images.Select(async (img, index) =>
                {
                    var extension = Path.GetExtension(img.FilePath);
                    var fileName = $"img_{index + 1}_{Guid.NewGuid():N}{extension}";
                    var blobPath = $"cars/{dealerFolder}/{carFolder}/{fileName}";

                    // 🔥 Compress image before upload
                    using var compressedStream = ImageHelper.CompressImage(img.FilePath, 70);

                    // 🔁 Retry logic
                    var url = await RetryHelper.RetryAsync(() =>
                        _blobService.UploadAsync(compressedStream, blobPath, "image/jpeg")
                    );

                    lock (uploadedUrls)
                    {
                        uploadedUrls.Add(url);
                        uploadedBlobs.Add(url);
                    }

                    // 📊 Progress update
                    Interlocked.Increment(ref completed);
                    var progress = (double)completed / total;

                    if (progressCallback != null)
                        await progressCallback(progress);
                });

                await Task.WhenAll(tasks);
            }
            catch
            {
                // ❌ Rollback uploaded images
                foreach (var blob in uploadedBlobs)
                {
                    await _blobService.DeleteAsync(blob);
                }

                throw;
            }

            // --------------------------------------
            // STEP 3: Update Car Images in DB
            // --------------------------------------
            var updateResponse = await _apiService
                .PutAsync<List<string>, bool>($"cars/{carId}/images", uploadedUrls);

            if (!updateResponse.Success)
                throw new Exception("Failed to update car images.");

            return updateResponse;
        }
    }
}
