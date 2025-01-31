using Avalonia.Controls;
using Avalonia.Interactivity;
using CountByRequest.ViewModels;

namespace CountByRequest.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        // Обрабатываем нажатие кнопки
        var viewModel = DataContext as MainViewModel;
        if (viewModel != null)
        {
            // Прячем кнопки
            viewModel.IsButtonEnabled = false;
            viewModel.Message = "Done";

            // TODO: посылаем JSON с результатами пересчёта на банковский сервер
        }
    }
}
