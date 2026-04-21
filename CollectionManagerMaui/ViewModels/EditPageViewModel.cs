using CollectionManagerMaui.Models;
using CollectionManagerMaui.Services;
using CollectionManagerMaui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CollectionManagerMaui.ViewModels
{
    [QueryProperty(nameof(Item), "Item")]
    [QueryProperty(nameof(Collection), "Collection")]
    public partial class EditPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private CollectionModel collection;

        [ObservableProperty]
        ItemModel item;


        [RelayCommand]
        async Task Save()
        {
            if (Collection?.Items != null)
            {
                var sortedItems = Collection.Items.OrderBy(item => item.State == "Sprzedane" ? 1 : 0).ToList();

                Collection.Items.Clear();

                foreach (var item in sortedItems)
                {
                    Collection.Items.Add(item);
                }
            }

            await FileService.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task AddImage(ItemModel item)
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Wybierz obraz"
            });
            if (result != null)
            {
                item.ImagePath = result.FullPath;
            }
        }
    }
}
