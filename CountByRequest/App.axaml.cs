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

namespace CountByRequest;

/*
public class MainWindowService
{
    public MainWindow MainWindow { get; set; }
}
*/

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
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        // Start the HTTP server
        StartHttpServer();

        base.OnFrameworkInitializationCompleted();
    }

    private void StartHttpServer()
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
                //services.AddSingleton<MainWindowService>(); // Register the MainWindowService
            })
            .Build();

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

/*
            // Access the IServiceProvider from the IApplicationBuilder
            var serviceProvider = app.ApplicationServices;

            // Resolve the MainWindowService
            var mainWindowService = serviceProvider.GetService<MainWindowService>();

            if (mainWindowService?.MainWindow != null)
            {
                var viewModel = mainWindowService.MainWindow.DataContext as MainViewModel;
                if (viewModel != null)
                {
                    // Update the ViewModel's Message property
                    viewModel.Message = "Updated from HTTP Request";
                }
            }
*/
        });
    }
}