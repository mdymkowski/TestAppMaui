using TestAppMaui.MauiClient.ViewModels;

namespace TestAppMaui.MauiClient.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
