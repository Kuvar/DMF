using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ViewState = DMF.Enums.ViewState;

namespace DMF.PageModels
{

    public partial class HomeViewModel : ObservableObject
    {
        private readonly ICarService _carService;

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

        private CarFilterModel _currentFilter;

        public HomeViewModel(ICarService carService)
        {
            CurrentState = ViewState.Loading;
            _carService = carService;
            _cars = new ObservableCollection<CarFilterResult>();
            _currentFilter = new CarFilterModel();
        }

        public void Initialize()
        {
            LoadCarsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadCars()
        {
            CurrentState = ViewState.Loading;
            //await Task.Delay(2000);

            Cars.Clear();
            _currentFilter.Page = 1;
            _currentFilter.PageSize = 20;

            _hasMoreData = true;
            _totalRecords = 0;

            await LoadNextPage();
            //var result = await _carService.GetFilteredCarsAsync(_currentFilter);
            //var cars = result.Data?.Items as IEnumerable<CarFilterResult> ?? Enumerable.Empty<CarFilterResult>();
            //Cars = new ObservableCollection<CarFilterResult>(cars);

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
        void CarDetail(CarModel model)
        {
            Shell.Current.GoToAsync("cardetails", new Dictionary<string, object>
            {
                {"carDetail", model   }
            });
        }

        [RelayCommand]
        void LikeUnlike(CarModel model)
        {

        }

        private async Task LoadNextPage()
        {
            if (IsLoadingMore)
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
