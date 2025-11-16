using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InventoryManager
{
    public class InventoryItem : INotifyPropertyChanged
    {
        private string name = string.Empty;
        private int quantity;
        private decimal price;
        private string category = string.Empty;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value && !string.IsNullOrWhiteSpace(value))
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Category
        {
            get { return category; }
            set
            {
                if (category != value && !string.IsNullOrWhiteSpace(value))
                {
                    category = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalValue));
                }
            }
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                if (price != value && value >= 0)
                {
                    price = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalValue));
                }
            }
        }

        public decimal TotalValue
        {
            get { return Quantity * Price; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}