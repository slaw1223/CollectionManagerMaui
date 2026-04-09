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

        public static ObservableCollection<CollectionModel> Collections { get; } = new ObservableCollection<CollectionModel>();

        public FileService()
        {}

        public static async Task SaveAsync()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var coll in Collections)
            {
                sb.AppendLine($"Collection: {coll.Name}");
                foreach (var i in coll.Items)
                    sb.AppendLine($"- {i.Name}|{i.Price}|{i.State}|{i.Rating}|{i.Comment}|{i.Rarity}|{i.Category}");
                sb.AppendLine();
            }

            var dir = Path.GetDirectoryName(FilePath) ?? FileSystem.AppDataDirectory;
            Directory.CreateDirectory(dir);
            await File.WriteAllTextAsync(FilePath, sb.ToString());
        }

        public static async Task LoadAsync()
        {
            Collections.Clear();

            if (!File.Exists(FilePath))
                return;

            Console.WriteLine($"File path: {FilePath}");

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
                    current = new CollectionModel { Name = collectionName };
                    Collections.Add(current);
                }
                else if (line.StartsWith("-") && current != null)
                {
                    var itemRaw = line.Substring(1).Trim();
                    var parts = itemRaw.Split('|');

                    string itemName = "";
                    itemName = parts[0].Trim();

                    int itemPrice = 0;
                    var p = parts[1].Trim();
                    if (int.TryParse(p, out var pInt))
                        itemPrice = pInt;

                    string itemState = "";
                    itemState = parts[2].Trim();

                    int itemRating = 0;
                    var r = parts[3].Trim();
                    if (int.TryParse(r, out var rInt))
                        itemRating = rInt;

                    string itemComment = "";
                    itemComment = parts[4].Trim();

                    string itemRarity = "";
                    itemRarity = parts[5].Trim();

                    string itemCategory = "";
                    itemCategory = parts[6].Trim();

                    var item = new ItemModel { Name = itemName, Price = itemPrice, State = itemState, Rating = itemRating, Comment = itemComment, Rarity = itemRarity, Category = itemCategory };
                    current.Items.Add(item);
                }
            }
        }

        public static async Task ClearAllAsync()
        {
            Collections.Clear();
            await SaveAsync();
        }
    }
}
