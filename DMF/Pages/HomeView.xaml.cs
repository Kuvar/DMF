namespace DMF.Pages;

public partial class HomeView : ContentView
{
    public HomeView(HomeViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        catch (Exception ex)
        {

        }
    }

    private async void LikeUnlikeCommand_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not VisualElement view)
            return;

        await TouchAnimation.AnimateAsync(view);

        if (view.BindingContext is CarModel car &&
            BindingContext is HomeViewModel vm &&
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
            BindingContext is HomeViewModel vm &&
            vm.CarDetailCommand.CanExecute(car))
        {
            vm.CarDetailCommand.Execute(car);
        }
    }

    private void HomeViewRoot_Loaded(object sender, EventArgs e)
    {
        if (BindingContext is HomeViewModel vm)
        {
            vm.Initialize(); // 🔥 THIS MAKES HOME VIEW LOAD FIRST TIME
        }
    }

    private void CarList_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (BindingContext is not HomeViewModel vm)
            return;

        if (e.VerticalDelta <= 0)
            return;

        // last visible index
        var lastVisible = e.LastVisibleItemIndex;

        // total items
        var total = vm.Cars.Count;

        // trigger when near bottom
        if (lastVisible >= total - 2)
        {
            vm.LoadMoreCarsCommand.Execute(null);
        }
    }
}