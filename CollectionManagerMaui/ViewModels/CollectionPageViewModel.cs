using CommunityToolkit.Mvvm.ComponentModel;
using CollectionManagerMaui.Models;
using CollectionManagerMaui.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionManagerMaui.Views;

namespace CollectionManagerMaui.ViewModels
{
    [QueryProperty(nameof(Collection), "Collection")]
    public partial class CollectionPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private CollectionModel collection;

        [ObservableProperty]
        private ItemModel newItem = new ItemModel();

        [ObservableProperty]
        private ItemModel item;


        [RelayCommand]
        async Task Save()
        {
            if (Collection.Items != null)
            {
                var sortedItems = Collection.Items.OrderBy(item => item.State == "Sprzedane" ? 1 : 0).ToList();

                Collection.Items.Clear();

                foreach (var item in sortedItems)
                {
                    Collection.Items.Add(item);
                }
            }

            await FileService.SaveAsync();
        }

        [RelayCommand]
        async Task EditItem(ItemModel item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync(nameof(EditPage), new Dictionary<string, object>
            {
                ["Item"] = item,
                ["Collection"] = Collection
            });
        }

        [RelayCommand]
        async Task RemoveItem(ItemModel item)
        {
            bool result = await App.Current.MainPage.DisplayAlert("Warning", $"Czy na pewno chcesz usunąć przedmiot '{item.Name}'?", "Tak", "Nie");
            if (result)
            {
                Collection.Items.Remove(item);
                await Save();
            }
        }

        [RelayCommand]
        async Task AddItem()
        {
            if (string.IsNullOrWhiteSpace(NewItem.Name))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Nazwa przedmiotu nie może być pusta", "OK");
                return;
            }
            if (!Collection.Items.Any(i => i.Name.Equals(NewItem.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                Collection.Items.Add(new ItemModel { Name = NewItem.Name.Trim(), Rating = 1 });
                NewItem.Name = string.Empty;
                await Save();
            }
            else
            {
                bool result = await App.Current.MainPage.DisplayAlert("Warning", "Taki przedmiot już istnieje", "Dodaj i tak", "Anuluj");
                if (result)
                {
                    int counter = 1;
                    string baseName = NewItem.Name.Trim();
                    while (Collection.Items.Any(i => i.Name.Equals($"{baseName} ({counter})", StringComparison.OrdinalIgnoreCase)))
                    {
                        counter++;
                    }
                    Collection.Items.Add(new ItemModel { Name = $"{baseName} ({counter})", Rating = 1 });
                    NewItem.Name = string.Empty;
                    await Save();
                }
            }
        }
    }
}
