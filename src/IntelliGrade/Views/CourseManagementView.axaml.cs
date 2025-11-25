using Avalonia.Controls;
using Avalonia.Interactivity;
using IntelliGrade.App.ViewModels;

namespace IntelliGrade.App.Views;

public partial class CourseManagementView : Window
{
    public CourseManagementView()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
