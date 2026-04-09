using CollectionManagerMaui.ViewModels;

namespace CollectionManagerMaui.Views;

public partial class EditPage : ContentPage
{
	public EditPage()
	{
		InitializeComponent();
        var vm = new EditPageViewModel();
        BindingContext = vm;
    }
}