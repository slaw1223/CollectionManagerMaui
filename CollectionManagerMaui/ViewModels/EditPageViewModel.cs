using CollectionManagerMaui.Models;
using CollectionManagerMaui.Services;
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
    [QueryProperty(nameof(Collection), "Collection")]
    public partial class EditPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private CollectionModel collection;

        [ObservableProperty]
        ItemModel newItem = new ItemModel();

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
                Collection.Items.Add(new ItemModel { Name = NewItem.Name.Trim() });
                NewItem.Name = string.Empty;
                await FileService.SaveAsync();
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
                    Collection.Items.Add(new ItemModel { Name = $"{baseName} ({counter})" });
                    NewItem.Name = string.Empty;
                    await FileService.SaveAsync();
                }
            }
        }

        [RelayCommand]
        async Task Save()
        {
            await FileService.SaveAsync();
        }

        [RelayCommand]
        async Task RemoveItem(ItemModel item)
        {
            bool result = await App.Current.MainPage.DisplayAlert("Warning", $"Czy na pewno chcesz usunąć przedmiot '{item.Name}'?", "Tak", "Nie");
            if (result)
            {
                Collection.Items.Remove(item);
                await FileService.SaveAsync();
            }
        }

        partial void OnCollectionChanged(CollectionModel value)
        {
            if (value == null)
                return;

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
    }
}
