namespace DMF.Pages;

public partial class ProfileViewPage : ContentPage
{
    private bool _hasAnimated;
    public ProfileViewPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewPageModel vm)
        {
            vm.Initialize();
        }
    }
}