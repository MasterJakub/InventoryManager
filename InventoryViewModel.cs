using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace InventoryManager
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Predicate<object?>? canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => canExecute == null || canExecute(parameter);

        public void Execute(object? parameter) => execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class InventoryViewModel : INotifyPropertyChanged
    {
        private readonly DataSaving dataService;

        public ObservableCollection<InventoryItem> InventoryItems { get; set; }

        public ObservableCollection<string> AvailableCategories { get; set; }

        #region Vstupní vlastnosti
        private string newItemName = string.Empty;
        public string NewItemName
        {
            get { return newItemName; }
            set { newItemName = value; OnPropertyChanged(); }
        }

        private string newItemCategory = string.Empty;
        public string NewItemCategory
        {
            get { return newItemCategory; }
            set { newItemCategory = value; OnPropertyChanged(); }
        }

        private int newItemQuantity;
        public int NewItemQuantity
        {
            get { return newItemQuantity; }
            set { newItemQuantity = value; OnPropertyChanged(); }
        }

        private decimal newItemPrice;
        public decimal NewItemPrice
        {
            get { return newItemPrice; }
            set { newItemPrice = value; OnPropertyChanged(); }
        }

        private int adjustmentAmount = 1;
        public int AdjustmentAmount
        {
            get { return adjustmentAmount; }
            set
            {
                if (value >= 0)
                {
                    adjustmentAmount = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private InventoryItem? selectedItem;
        public InventoryItem? SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public ICommand AddItemCommand { get; private set; }
        public ICommand DeleteItemCommand { get; private set; }
        public ICommand AdjustQuantityCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public InventoryViewModel()
        {
            dataService = new DataSaving();
            InventoryItems = dataService.LoadInventory();

            AvailableCategories = new ObservableCollection<string>();
            LoadAvailableCategories();

            // POUŽÍVÁ RelayCommand
            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            DeleteItemCommand = new RelayCommand(DeleteItem, CanDeleteItem);
            AdjustQuantityCommand = new RelayCommand(AdjustQuantity, CanAdjustQuantity);
            SaveCommand = new RelayCommand(UlozitData);
        }

        private void LoadAvailableCategories()
        {
            var categories = InventoryItems
                                .Select(item => item.Category)
                                .Distinct()
                                .OrderBy(c => c);

            AvailableCategories.Clear();
            foreach (var category in categories)
            {
                if (!string.IsNullOrWhiteSpace(category))
                {
                    AvailableCategories.Add(category);
                }
            }
        }

        private void UlozitData(object? parameter)
        {
            dataService.SaveInventory(InventoryItems);
        }

        private void AddItem(object? parameter)
        {
            if (CanAddItem(parameter))
            {
                var newItem = new InventoryItem
                {
                    Name = NewItemName,
                    Category = NewItemCategory,
                    Quantity = NewItemQuantity,
                    Price = NewItemPrice
                };
                InventoryItems.Add(newItem);

                if (!string.IsNullOrWhiteSpace(newItem.Category) && !AvailableCategories.Contains(newItem.Category))
                {
                    AvailableCategories.Add(newItem.Category);
                }

                NewItemName = string.Empty;
                NewItemCategory = string.Empty;
                NewItemQuantity = 0;
                NewItemPrice = 0.00m;
            }
        }

        private bool CanAddItem(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(NewItemName) &&
                   !string.IsNullOrWhiteSpace(NewItemCategory) &&
                   NewItemQuantity > 0 &&
                   NewItemPrice >= 0;
        }

        private void DeleteItem(object? parameter)
        {
            if (parameter is InventoryItem itemToDelete)
            {
                InventoryItems.Remove(itemToDelete);
            }
        }

        private bool CanDeleteItem(object? parameter)
        {
            return parameter is InventoryItem;
        }

        private void AdjustQuantity(object? parameter)
        {
            if (parameter is object[] parameters && parameters.Length == 2 &&
                parameters[0] is InventoryItem selectedItem &&
                parameters[1] is string operation)
            {
                int delta = AdjustmentAmount;
                if (operation == "Remove") delta = -delta;

                if (CanAdjustQuantity(parameters))
                {
                    selectedItem.Quantity += delta;
                }
            }
        }

        private bool CanAdjustQuantity(object? parameter)
        {
            if (parameter is object[] parameters && parameters.Length == 2 &&
                parameters[0] is InventoryItem selectedItem &&
                parameters[1] is string operation)
            {
                if (AdjustmentAmount <= 0) return false;

                if (operation == "Remove")
                {
                    return selectedItem.Quantity >= AdjustmentAmount;
                }
                return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}