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
    public partial class EditPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private CollectionModel collection;

        [ObservableProperty]
        ItemModel item;


        [RelayCommand]
        async Task Save()
        {
            await FileService.SaveAsync();
        }


        partial void OnCollectionChanged(CollectionModel value)
        {
            if (value == null)
                return;

            if (value.Items.Count == 0)
            {
                var defaultItem = new ItemModel
                {
                    Name = "Przedmiot 1"
                };

                value.Items.Add(defaultItem);
                Subscribe(defaultItem);

                _ = Save();
            }

            foreach (var item in value.Items)
                Subscribe(item);

            value.Items.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (ItemModel item in e.NewItems)
                        Subscribe(item);
            };
        }
        void Subscribe(ItemModel item)
        {
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ItemModel.State))
                {
                    if (item.State == "Sprzedane")
                    {
                        MoveToEnd(item);
                    }
                }
            };
        }
        void MoveToEnd(ItemModel item)
        {
            if (Collection.Items.Remove(item))
            {
                Collection.Items.Add(item);
            }
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
                await Save();
            }
        }
    }
}
