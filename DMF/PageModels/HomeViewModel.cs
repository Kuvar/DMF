using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ViewState = DMF.Enums.ViewState;

namespace DMF.PageModels
{

    public partial class HomeViewModel : ObservableObject
    {
        private CarFilterModel _currentFilter;
        private readonly ICarService _carService;
        private readonly ISecureStorageService _storageService;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<CarFilterResult> _cars;

        [ObservableProperty]
        private bool isLoadingMore;

        [ObservableProperty]
        private bool canLoadMore;


        private bool _hasMoreData = true;
        private int _totalRecords;

        [ObservableProperty]
        private ViewState currentState = ViewState.Loading;



        public HomeViewModel(ICarService carService, ISecureStorageService secureStorage)
        {
            CurrentState = ViewState.Loading;
            _carService = carService;
            _storageService = secureStorage;
            _cars = new ObservableCollection<CarFilterResult>();
            _currentFilter = new CarFilterModel();

            var userIdTask = _storageService.GetAsync(AppConstants.UserId);
            userIdTask.Wait();
            var userIdString = userIdTask.Result;
            if (int.TryParse(userIdString, out var userId))
            {
                _currentFilter.UserDetailID = userId;
            }
            else
            {
                _currentFilter.UserDetailID = 0;
            }
        }

        public void Initialize()
        {
            LoadCarsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadCars()
        {
            CurrentState = ViewState.Loading;

            Cars.Clear();
            _currentFilter.Page = 1;
            _currentFilter.PageSize = 20;

            _hasMoreData = true;
            _totalRecords = 0;

            await LoadNextPage();
            CurrentState = ViewState.Success;
        }

        [RelayCommand]
        private async Task LoadMoreCars()
        {
            Debug.WriteLine($"LoadMore fired — Cars:{Cars.Count}");

            if (!CanLoadMore)
                return;

            if (IsLoadingMore)
                return;

            if (!_hasMoreData)
                return;

            if (_totalRecords == 0)
                return;

            await LoadNextPage();
        }


        [RelayCommand] void Filter() { }
        [RelayCommand] void Sort() { }
        [RelayCommand] void Brand() { }
        [RelayCommand] void Model() { }

        [RelayCommand]
        void CarDetail(CarFilterResult model)
        {
            Shell.Current.GoToAsync("cardetails", new Dictionary<string, object>
            {
                {"carDetail", model   }
            });
        }

        [RelayCommand]
        async Task LikeUnlike(CarFilterResult model)
        {
            try
            {
                var response = await _carService.ToggleWishlistAsync(7, model.ID);

                if (response.Success)
                {
                    var car = Cars.FirstOrDefault(x => x.ID == model.ID);
                    if (car != null)
                        car.IsWishlisted = response.Data;
                    OnPropertyChanged("Cars");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task LoadNextPage()
        {
            if (IsLoadingMore || _currentFilter.UserDetailID == 0)
                return;

            try
            {
                IsLoadingMore = true;

                var result = await _carService.GetFilteredCarsAsync(_currentFilter);

                var page = result.Data;
                if (page == null)
                    return;

                _totalRecords = page.TotalRecords;
                if (Cars.Count == 0)
                    Cars = new ObservableCollection<CarFilterResult>(page.Items);
                else
                    foreach (var car in page.Items)
                        Cars.Add(car);

                _currentFilter.Page++;

                _hasMoreData = Cars.Count < _totalRecords;
                CanLoadMore = Cars.Count >= _currentFilter.PageSize;
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

    }
}
