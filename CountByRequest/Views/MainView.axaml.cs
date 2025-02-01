using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CountByRequest.Utilites;
using CountByRequest.ViewModels;
using System.Threading;
using System.Threading.Tasks;

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
            viewModel.Message = "Sending...";

            // Посылаем JSON с результатами пересчёта на банковский сервер.
            Thread postRequestThread = new Thread(() => SendPost());
            postRequestThread.Start();
        }
    }

    public async Task SendPost()
    {
        var postSender = new PostSender();
        var postRequest = new PostRequest
        {
            Currency = "USD",
            Notes = 50,
            Denomination = 100
        };

        // Предполагаем, что сервер находится на этом же компьютере и слушает порт 5000
        string url = "http://localhost:5000/api/data";
        var result = await postSender.SendPostAsync(postRequest, url);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null) {
                viewModel.Message = result;
            }
        });
    }
}
