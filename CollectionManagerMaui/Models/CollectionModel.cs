using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManagerMaui.Models
{
    public partial class CollectionModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        public ObservableCollection<ItemModel> Items { get; set; } = new();
    }
}
