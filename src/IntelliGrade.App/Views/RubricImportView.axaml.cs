using Avalonia.Controls;
using Avalonia.Interactivity;
using IntelliGrade.App.ViewModels;

namespace IntelliGrade.App.Views;

public partial class RubricImportView : Window
{
    public RubricImportView()
    {
        InitializeComponent();

        // Wire up storage provider when window opens
        Opened += (s, e) =>
        {
            if (DataContext is RubricImportViewModel vm)
            {
                vm.StorageProvider = StorageProvider;

                // Close window on successful import
                vm.RubricImported += (_, _) =>
                {
                    Close();
                };
            }
        };
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
