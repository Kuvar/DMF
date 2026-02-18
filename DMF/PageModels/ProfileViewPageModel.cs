using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DMF.PageModels
{
    public partial class ProfileViewPageModel : ObservableObject
    {
        [ObservableProperty]
        private bool isLoanSelected = false;

        [ObservableProperty]
        private bool isRegistrationSelected = false;

        [ObservableProperty]
        private bool isNocSelected = false;

        public ProfileViewPageModel()
        {

        }

        public void Initialize()
        {

        }

        [RelayCommand] Task Back() => Shell.Current.GoToAsync("..", true);

        [RelayCommand]
        void SelectService(string param)
        {
            switch (param)
            {
                case "Loan":
                    IsLoanSelected = !IsLoanSelected;
                    break;
                case "Registration":
                    IsRegistrationSelected = !IsRegistrationSelected;
                    break;
                case "NOC":
                    IsNocSelected = !IsNocSelected;
                    break;
            }
        }
    }
}
