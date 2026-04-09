using CollectionManagerMaui.ViewModels;

namespace CollectionManagerMaui.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        var vm = new MainPageViewModel();
        BindingContext = vm;
    }
}