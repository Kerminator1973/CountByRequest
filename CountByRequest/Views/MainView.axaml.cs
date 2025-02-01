using Avalonia.Controls;
using Avalonia.Interactivity;
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
            viewModel.Message = "Done";

            // Посылаем JSON с результатами пересчёта на банковский сервер.
            
            // TODO: сейчас мы не ждём завершения запроса, но нам будет нужно
            // уведомить оператора о том, получилось ли выполнить операцию, или нет
            Thread postRequestThread = new Thread(() => SendPost());
            postRequestThread.Start();
        }
    }

    public async Task SendPost()
    {
        var poseSender = new PostSender();
        var poseRequest = new PostRequest
        {
            Currency = "USD",
            Notes = 50,
            Denomination = 100
        };

        // Предполагаем, что сервер находится на этом же компьютере и слушает порт 5000
        string url = "http://localhost:5000";
        await poseSender.SendPostAsync(poseRequest, url);
    }
}
