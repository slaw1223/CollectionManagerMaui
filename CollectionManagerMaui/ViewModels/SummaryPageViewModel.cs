using CollectionManagerMaui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManagerMaui.ViewModels
{
    [QueryProperty(nameof(Collection), "Collection")]
    public partial class SummaryPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private CollectionModel collection;

        public int SoldItemsCount =>
        Collection?.Items.Count(i => i.State == "Sprzedane") ?? 0;

        public int ForSaleItemsCount =>
            Collection?.Items.Count(i => i.State == "Na sprzedaż") ?? 0;

        public int OwnedItemsCount =>
            (Collection?.Items.Count ?? 0) - SoldItemsCount;

        partial void OnCollectionChanged(CollectionModel value)
        {
            OnPropertyChanged(nameof(SoldItemsCount));
            OnPropertyChanged(nameof(ForSaleItemsCount));
            OnPropertyChanged(nameof(OwnedItemsCount));
        }
    }
}
