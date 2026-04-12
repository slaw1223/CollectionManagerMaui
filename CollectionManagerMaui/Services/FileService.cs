using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionManagerMaui.Models;

namespace CollectionManagerMaui.Services
{
    public class FileService
    {
        const string FileName = "collections.txt";
        static string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        public static ObservableCollection<CollectionModel> Collections { get; } = new();

        public FileService()
        {}

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
                    sb.AppendLine($"- {i.Name}|{i.Price}|{i.State}|{i.Rating}|{i.Comment}|{i.Rarity}|{i.Category}");
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

        public static async Task LoadAsync()
        {
            try
            {
                Collections.Clear();

            if (!File.Exists(FilePath))
                return;

            Debug.WriteLine($"File path: {FilePath}");


                var lines = await File.ReadAllLinesAsync(FilePath);

                CollectionModel? current = null;

                

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
                        current = new CollectionModel { Name = collectionName, Items = new ObservableCollection<ItemModel>() };
                        Collections.Add(current);
                    }
                    else if (line.StartsWith("-") && current != null)
                    {
                        var itemRaw = line.Substring(1).Trim();
                        var parts = itemRaw.Split('|');

                        if (parts.Length < 7)
                            continue;

                        string itemName = parts[0].Trim();

                        int itemPrice = 0;
                        var p = parts[1].Trim();
                        if (int.TryParse(p, out var pInt))
                            itemPrice = pInt;

                        string itemState = parts[2].Trim();

                        int itemRating = 0;
                        var r = parts[3].Trim();
                        if (int.TryParse(r, out var rInt))
                            itemRating = rInt;

                        string itemComment = parts[4].Trim();

                        string itemRarity = parts[5].Trim();

                        string itemCategory = parts[6].Trim();

                        var item = new ItemModel { Name = itemName, Price = itemPrice, State = itemState, Rating = itemRating, Comment = itemComment, Rarity = itemRarity, Category = itemCategory };
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
    }
}
