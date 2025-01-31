using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using CountByRequest.ViewModels;
using CountByRequest.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Threading;

namespace CountByRequest;


// Сервис, основная задача которого обеспечивать доступ из потока обработки http-запросов (ASP.NET Core)
// к потоку пользовательского интерфейса (Avalonia)
public class MainWindowService
{
    public required MainWindow MainWindow { get; set; }
}


public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            desktop.MainWindow = mainWindow;

            // Запускаем HTTP-сервер и передаём ссылку на главное окно приложения Avalonia
            StartHttpServer(mainWindow);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void StartHttpServer(MainWindow mainWindow)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                    // Устанавливаем порт на котором мы будем слушать http-запросы
                    .UseUrls("http://0.0.0.0:8080") 
                    .UseStartup<Startup>();
            })
            .ConfigureServices(services =>
            {
                // Регистрируем сервис (DI) для передачи ссылки на главное окно
                // в ASP.NET Core
                services.AddSingleton<MainWindowService>();
            })
            .Build();

        // Получаем сервис и сохраняем в нём ссылку на главное окно приложения
        var mainWindowService = host.Services.GetService<MainWindowService>();
        if (mainWindowService != null)
        {
            mainWindowService.MainWindow = mainWindow;
        }

        host.Start();
    }
}

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            // Обрабатываем Http-запрос, например: http://localhost:8080
            await context.Response.WriteAsync("Hello from Avalonia HTTP Server!");

            // Запускаем код в потоке пользовательского интерфейса
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Получаем ссылку на главное окно приложения
                var mainWindowService = app.ApplicationServices.GetService<MainWindowService>();
                if (mainWindowService?.MainWindow != null)
                {
                    var viewModel = mainWindowService.MainWindow.DataContext as MainViewModel;
                    if (viewModel != null)
                    {
                        // Изменяем свойство модели, которое должно сразу же отразиться
                        // в пользовательском интерфейсе
                        viewModel.Message = "Updated from HTTP Request";
                    }
                }
            });
        });
    }
}