using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            _ = FileService.LoadAsync(false, "");
        }

        [RelayCommand]
        async Task AddCollection()
        {
            var name = NewCollection.Name ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Nazwa kolekcji nie może być pusta", "OK");
                return;
            }

            var trimmed = name.Trim();

            if (!Collections.Any(c => c.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
            {
                Collections.Add(new CollectionModel { Name = trimmed });
                NewCollection = new CollectionModel();
                await FileService.SaveAsync();
            }
            else
            {
                bool result = await App.Current.MainPage.DisplayAlert("Warning", $"Kolekcja o nazwie '{trimmed}' już istnieje", "Dodaj i tak", "Anuluj");

                if (result)
                {
                    int counter = 1;
                    string baseName = trimmed;
                    string newName;

                    do
                    {
                        newName = $"{baseName}({counter})";
                        counter++;
                    }
                    while (Collections.Any(c => c.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)));

                    Collections.Add(new CollectionModel { Name = newName });
                    NewCollection = new CollectionModel();
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
            if(collection == null)
                return;
            var name = collection.Name ?? "???";

            bool result = await App.Current.MainPage.DisplayAlert("Warning", $"Czy na pewno chcesz usunąć kolekcję '{name}'?", "Tak", "Nie");
            if (result)
            {
                var toRemove = collection;

                await Task.Delay(50);

                Debug.WriteLine($"Removing: {toRemove.Name}");
                Collections.Remove(toRemove);

                await FileService.SaveAsync();
            }
        }

        [RelayCommand]
        async Task ExportCollection(CollectionModel collection)
        {
            if (collection == null)
                return;
            await FileService.ExportAsync(collection);
        }

        [RelayCommand]
        async Task ImportCollection()
        {
            await FileService.ImportAsync();
        }
    }
}
