using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System;

namespace InventoryManager
{
    public class DataSaving
    {
        private readonly string filePath;

        public DataSaving()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "WPFSpravceInventare");
            Directory.CreateDirectory(appFolder);

            filePath = Path.Combine(appFolder, "inventory.json");
        }

        public ObservableCollection<InventoryItem> LoadInventory()
        {
            if (!File.Exists(filePath))
            {
                return new ObservableCollection<InventoryItem>
                {
                    new InventoryItem { Name = "Laptop", Category = "Electronics", Quantity = 10, Price = 1200.00m },
                    new InventoryItem { Name = "Monitor", Category = "Electronics", Quantity = 25, Price = 350.50m },
                    new InventoryItem { Name = "Stapler", Category = "Office Supplies", Quantity = 50, Price = 15.00m }
                };
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var items = JsonSerializer.Deserialize<List<InventoryItem>>(json);

                if (items != null)
                {
                    return new ObservableCollection<InventoryItem>(items);
                }
                else
                {
                    return new ObservableCollection<InventoryItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání inventáře: {ex.Message}");
                return new ObservableCollection<InventoryItem>();
            }
        }

        public void SaveInventory(IEnumerable<InventoryItem> items)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(items, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání inventáře: {ex.Message}");
            }
        }
    }
}