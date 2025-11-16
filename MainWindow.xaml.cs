using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace InventoryManager
{
    // --- Konvertory ---
    // Převádí "nic" (null) na "pravda/nepravda" pro aktivaci tlačítek
    public class PrevodnikNullNaBool : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Seskupuje více parametrů (položku a operaci) do jednoho balíčku pro příkaz
    public class PrevodnikPrikazuUpravy : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetTypes, object? parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] != DependencyProperty.UnsetValue && values[1] is string operation)
            {
                return new object[] { values[0], operation };
            }
            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // TOTO JE KLÍČ K FUNKČNOSTI: Vytvoří ViewModel a propojí s XAML.
            this.DataContext = new InventoryViewModel();

            // Původní složitá metoda pro odznačování je odstraněna,
            // protože způsobovala chyby a pád aplikace. Zůstává pouze nezbytný kód.
        }
    }
}