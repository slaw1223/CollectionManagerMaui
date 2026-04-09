using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManagerMaui.Models
{
    public partial class ItemModel : ObservableObject
    {
        [ObservableProperty] private string name;
        [ObservableProperty] private int price;
        [ObservableProperty] private string state;
        [ObservableProperty] private int rating;
        [ObservableProperty] private string comment;
        [ObservableProperty] private string rarity;
        [ObservableProperty] private string category;
    }
}
