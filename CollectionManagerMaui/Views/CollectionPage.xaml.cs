using CollectionManagerMaui.ViewModels;

namespace CollectionManagerMaui.Views;

public partial class CollectionPage : ContentPage
{
	public CollectionPage()
	{
		InitializeComponent();
        var vm = new CollectionPageViewModel();
        BindingContext = vm;
    }
}