namespace DMF.Pages;

[QueryProperty(nameof(Car), "Car")]
public partial class AddImageUploadPage : ContentPage
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

    public AddImageUploadPage()
    {
        InitializeComponent();
    }
}