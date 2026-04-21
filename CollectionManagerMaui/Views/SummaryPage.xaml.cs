using CollectionManagerMaui.ViewModels;
namespace CollectionManagerMaui.Views;

public partial class SummaryPage : ContentPage
{
	public SummaryPage()
	{
		InitializeComponent();
        var vm = new SummaryPageViewModel();
        BindingContext = vm;
    }
}