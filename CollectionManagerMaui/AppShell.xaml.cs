using CollectionManagerMaui.Views;

namespace CollectionManagerMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(CollectionPage), typeof(CollectionPage));
            Routing.RegisterRoute(nameof(EditPage), typeof(EditPage));
            Routing.RegisterRoute(nameof(SummaryPage), typeof(SummaryPage));
        }
    }
}
