namespace DMF.Pages;

public partial class FavoriteView : ContentView
{
    public FavoriteView(FavoriteViewModel vm)
    {
        InitializeComponent();

        this.BindingContext = vm;
    }

    private async void LikeUnlikeCommand_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not VisualElement view)
            return;

        await TouchAnimation.AnimateAsync(view);

        if (view.BindingContext is CarModel car &&
            BindingContext is FavoriteViewModel vm &&
            vm.LikeUnlikeCommand.CanExecute(car))
        {
            vm.LikeUnlikeCommand.Execute(car);
        }
    }

    private async void CarDetailCommand_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not VisualElement view)
            return;

        await TouchAnimation.AnimateAsync(view);

        if (view.BindingContext is CarModel car &&
            BindingContext is FavoriteViewModel vm &&
            vm.CarDetailCommand.CanExecute(car))
        {
            vm.CarDetailCommand.Execute(car);
        }
    }

    private void FavoriteViewRoot_Loaded(object sender, EventArgs e)
    {
        if (BindingContext is FavoriteViewModel vm)
        {
            vm.Initialize();
        }
    }
}