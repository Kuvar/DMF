namespace DMF.Pages;

[QueryProperty(nameof(Car), "Car")]
public partial class AddCarStep2Page : ContentPage
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

    public AddCarStep2Page()
    {
        InitializeComponent();
    }
}