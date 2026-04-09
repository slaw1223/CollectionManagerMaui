using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionManagerMaui.Models;
using CollectionManagerMaui.Services;
using CollectionManagerMaui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CollectionManagerMaui.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        CollectionModel newCollection = new CollectionModel();

        public ObservableCollection<CollectionModel> Collections => FileService.Collections;

        public MainPageViewModel()
        {
            _ = FileService.LoadAsync();
        }

        [RelayCommand]
        async Task AddCollection()
        {
            if(NewCollection.Name.Equals("ClearAll", StringComparison.OrdinalIgnoreCase))
            {
                bool result = await App.Current.MainPage.DisplayAlert("Warning", "Czy na pewno chcesz usunąć wszystkie kolekcje?", "Tak", "Nie");
                if (result)
                {
                    NewCollection.Name = string.Empty;
                    await FileService.ClearAllAsync();
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(NewCollection.Name))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Nazwa kolekcji nie może być pusta", "OK");
                return;
            }

            if(!Collections.Any(c => c.Name.Equals(NewCollection.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                Collections.Add(new CollectionModel { Name = NewCollection.Name.Trim() });
                NewCollection.Name = string.Empty;
                await FileService.SaveAsync();
            }
            else
            {
                bool result = await App.Current.MainPage.DisplayAlert("Warning", "Taka kolekcja już istnieje", "Dodaj i tak", "Anuluj");

                if (result)
                {
                    int counter = 1;
                    string baseName = NewCollection.Name.Trim();
                    string newName;

                    do
                    {
                        newName = $"{baseName}({counter})";
                        counter++;
                    }
                    while (Collections.Any(c => c.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)));

                    Collections.Add(new CollectionModel { Name = newName });
                    NewCollection.Name = string.Empty;
                    await FileService.SaveAsync();
                }
            }
        }
        [RelayCommand]
        async Task RedirectToEdit(CollectionModel collection)
        {
            if (collection == null)
                return;

            await Shell.Current.GoToAsync(nameof(EditPage), new Dictionary<string, object>
            {
                ["Collection"] = collection
            });
        }
        [RelayCommand]
        async Task RemoveCollection(CollectionModel collection)
        {
            bool result = await App.Current.MainPage.DisplayAlert("Warning", $"Czy na pewno chcesz usunąć kolekcję '{collection.Name}'?", "Tak", "Nie");
            if (result)
            {
                Collections.Remove(collection);
                await FileService.SaveAsync();
            }
        }
    }
}
