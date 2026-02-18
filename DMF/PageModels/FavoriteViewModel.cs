using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ViewState = DMF.Enums.ViewState;

namespace DMF.PageModels
{
    public partial class FavoriteViewModel : ObservableObject
    {
        private readonly ICarService _carService;

        [ObservableProperty]
        private ObservableCollection<CarModel> _cars;

        [ObservableProperty]
        private ViewState currentState = ViewState.Loading;

        public FavoriteViewModel(ICarService carService)
        {
            CurrentState = new ViewState();
            _carService = carService;
            Cars = new ObservableCollection<CarModel>();
        }

        [RelayCommand]
        private async Task LoadCars()
        {
            CurrentState = ViewState.Loading;
            await Task.Delay(1000);

            var result = await _carService.GetFavoriteCarsAsync(1);
            Cars = new ObservableCollection<CarModel>(result);

            CurrentState = ViewState.Success;
        }

        public void Initialize()
        {
            LoadCarsCommand.Execute(null);
        }

        [RelayCommand]
        void LikeUnlike(CarModel model)
        {

        }

        [RelayCommand]
        void CarDetail(CarModel model)
        {
            Shell.Current.GoToAsync("cardetails", new Dictionary<string, object>
            {
                {"carDetail", model   }
            });
        }
    }
}
