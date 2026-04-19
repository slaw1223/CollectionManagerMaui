using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionManagerMaui.Models;
using CommunityToolkit.Maui.Storage;

namespace CollectionManagerMaui.Services
{
    public class FileService
    {
        private readonly IFolderPicker folderPicker;

        const string FileName = "collections.txt";
        static string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        public static ObservableCollection<CollectionModel> Collections { get; } = new();

        public FileService(IFolderPicker folderPicker)
        {
            this.folderPicker = folderPicker;
        }

        public static async Task SaveAsync()
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                foreach (var coll in Collections)
                {
                    sb.AppendLine($"Collection: {coll.Name}");

                    if (coll.Items == null)
                        continue;

                    foreach (var i in coll.Items)
                    sb.AppendLine($"- {i.Name}|=|{i.Price}|=|{i.State}|=|{i.Rating}|=|{i.Comment}|=|{i.Rarity}|=|{i.Category}|=|{i.ImagePath}");
                    sb.AppendLine();
                }

                var dir = Path.GetDirectoryName(FilePath) ?? FileSystem.AppDataDirectory;
                Directory.CreateDirectory(dir);

                await File.WriteAllTextAsync(FilePath, sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving file: {ex.Message}");
            }
        }

        public static async Task LoadAsync(bool isImport, string path)
        {
            try
            {
                if (!isImport)
                    Collections.Clear();

                if (!File.Exists(FilePath))
                return;

                Debug.WriteLine($"File path: {FilePath}");


                var lines = await File.ReadAllLinesAsync(FilePath);

                if(isImport)
                    lines = await File.ReadAllLinesAsync(path);

                CollectionModel current = null;

                foreach (var raw in lines)
                {
                    var line = raw?.Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        current = null;
                        continue;
                    }

                    if (line.StartsWith("Collection:"))
                    {
                        var collectionName = line.Substring("Collection:".Length).Trim();

                        if (isImport && Collections.Any(c => c.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase)))
                        {
                            bool result = await App.Current.MainPage.DisplayAlert("Warning",$"Kolekcja o nazwie '{collectionName}' już istnieje.", "Dodaj i tak", "Anuluj");

                            if (result)
                            {
                                int counter = 1;
                                string baseName = collectionName;
                                string newName;

                                do
                                {
                                    newName = $"{baseName}({counter})";
                                    counter++;
                                }
                                while (Collections.Any(c => c.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)));

                                collectionName = newName;
                            }
                            else
                            {
                                current = null;
                                continue;
                            }
                        }

                        current = new CollectionModel
                        {
                            Name = collectionName,
                            Items = new ObservableCollection<ItemModel>()
                        };

                        Collections.Add(current);
                    }
                    else if (line.StartsWith("-") && current != null)
                    {
                        var itemRaw = line.Substring(1).Trim();
                        var parts = itemRaw.Split("|=|");

                        if (parts.Length < 7)
                            continue;

                        string itemName = parts[0].Trim();

                        string itemPrice = parts[1].Trim();

                        string itemState = parts[2].Trim();

                        int itemRating = 0;
                        var r = parts[3].Trim();
                        if (int.TryParse(r, out var rInt))
                            itemRating = rInt;

                        string itemComment = parts[4].Trim();

                        string itemRarity = parts[5].Trim();

                        string itemCategory = parts[6].Trim();

                        string itemImagePath = parts[7].Trim();

                        var item = new ItemModel { Name = itemName, Price = itemPrice, State = itemState, Rating = itemRating, Comment = itemComment, Rarity = itemRarity, Category = itemCategory, ImagePath = itemImagePath };
                        current.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading file: {ex.Message}");
                return;
            }
        }

        public static async Task ExportAsync(CollectionModel collection)
        {
            try
            {
                var result = await FolderPicker.Default.PickAsync(CancellationToken.None);
                if (result.IsSuccessful)
                {
                    string folderPath = result.Folder.Path;
                    string exportPath = Path.Combine(folderPath, $"{collection.Name}.txt");


                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"Collection: {collection.Name}");

                    if (collection.Items == null)
                        return;

                    foreach (var i in collection.Items)
                        sb.AppendLine($"- {i.Name}|=|{i.Price}|=|{i.State}|=|{i.Rating}|=|{i.Comment}|=|{i.Rarity}|=|{i.Category}|=|{i.ImagePath}");
                    sb.AppendLine();

                    await File.WriteAllTextAsync(exportPath, sb.ToString());

                    await App.Current.MainPage.DisplayAlert("Info", $"Wyekportowano plik do: {exportPath}","Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error exporting file: {ex.Message}");
            }
        }

        public static async Task ImportAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();

                if (result == null)
                    return;

                await LoadAsync(true, result.FullPath);
                await SaveAsync();                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error importing file: {ex.Message}");
            }
        }
    }
}
