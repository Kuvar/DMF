namespace DMF.Pages;

[QueryProperty(nameof(Car), "Car")]
public partial class AddCarStep3Page : ContentPage
{
    public AddCarModel Car
    {
        set
        {
            BindingContext = new AddCarViewModel
            {
                Car = value
            };
        }
    }

    public AddCarStep3Page()
    {
        InitializeComponent();
    }
}