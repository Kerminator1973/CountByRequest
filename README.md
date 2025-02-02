# CountByRequest - пример реализации приложения для получения результатов дневной выручки

Приведённый демонстрационный пример иллюстрирует возможность отправить из банковской системы запрос начала пересчёта наличных.

Для проведения демонстрации необходимо:

- запустить имитацию банковской системы (web-сервер на базе Node.js), в которую будут передаваться результаты пересчёта в виде JSON-документа
- запустить desktop-приложение CountByRequest
- запустить браузер (локально) и в строке ввода url ввести: `http://localhost:8080'

Встроенный в desktop-приложение CountByRequest web-сервер обработает запрос и выведет на экран кнопку для завершения пересчёта. Как только кассир закончит пересчёт, этот пересчёт будет направлен в эмулятор банковской системы, как POST-запрос с JSON-документом: `http://localhost:5000/api/data`

## Что отсутствует в демонстрационной системе

Не осуществляется подключение к реальной базе пересчётов, т.к. это требует развертывания стенда и использования дополнительного оборудования для наполнения базы данных.

Структура JSON-документа не соответствует той, которая будет при промышленной эксплуатации - протоколы взаимодействия компонентов системы нуждаются в согласовании.

## Запуск простейшего web-сервера, который имитирует банковскую систему

В папке "serverExpress" находится исходный код на Node.js, который имитирует банковский сервер. Код приложения приведён в файле "server.js". Чтобы запустить сервер необходимо установить Node.js, скачать зависимости и запусить сервер.

Скачать Node.js можно с [официального сайта](https://nodejs.org/en) проекта. Текущая LTS-версия 22.13.1. Следует заметить, что эта сборка не работает на Windows 7 - необходимо использовать Windows 10/11, или Linux.

Запросить версию установленной среды исполнения Node.js:

```shell
node --version
```

Для того, чтобы установить зависимости достаточно выполнить команду:

```shell
npm install
```

Запустить приложение можно командой:

```shell
node server.js
```

В случае, если компания использует прокси-сервер, может потребоваться настроить подключение:

```shell
npm config set proxy http://proxy-server:port
npm config set https-proxy http://proxy-server:port
```

В приведённых выше командах нужно заменить `proxy-server:port` на реальные значения.

Посмотреть корректность настройки можно командами:

```shell
npm config get proxy
npm config get https-proxy
```

При получении запроса, web-сервер выведет информацию о нём в консоль, в которой сервер был запущен.

## сборка автономного приложения

Для сборки автономного Desktop-приложения следует в папке "CountByRequest.Desktop" выполнить команду:

```shell
dotnet publish -c Release -r win-x64 --self-contained
```

Результат сборки должен появиться в папке "\CountByRequest.Desktop\bin\Release\net8.0\win-x64\publish".

## Кратко об архитектуре desktop-приложения

Для создания пользовательского интерфейса используется framework Avalonia.

Для обработки внешних http-запросов, в приложение встроен web-сервер Kestrel и компоненты ASP.NET Core.

Все компоненты являются open-source. Система может работать под Windows 10/11 и Linux.

Приложение было создано по типовому шаблону desktop-приложений Avalonia.

Метапакет Microsoft.AspNetCore.App для ASP.NET Core добавляет функционал web-сервера. Запуск Kestrel осуществляется в отдельном методе - StartHttpServer():

```csharp
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
```

При конфигурации сервиса можно указать порт, который будет слушать сервер Kestrel:

```csharp
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
```

Критически важным является строка с указанием порта и адреса сети, с которой будут приниматься запросы:

```csharp
.UseUrls("http://0.0.0.0:8080")
```

Адрес `0.0.0.0` означает, что запросы будут приниматься с любого другого компьютера в подсети. Если бы мы указали `127.0.0.1`, или `localhost`, то запросы с внешних IP-адресов не обрабатывались бы.

В приведённом выше коде сохраняется ссылка на экземпляр главного окна приложения. Ссылка сохраняется в сервисе, который доступен через механизм Dependancy Injection. Ссылка на главное окно необходима для того, чтобы при получении http-запроса web-сервером можно было бы установливать атрибуты ViewModel, изменяя пользовательский интерфейс приложения Avalonia.

Важно обратить внимание на тот факт, что http-запросы обрабатываются в отдельном рабочем потоке. Попытка напрямую изменять атрибуты ViewModel приводит к возникновению исключения. Чтобы добиться желаемого результата, необходимо выполнять изменения пользовательского интерфейса в потоке пользовательского интерфейса. Например:

```csharp
await Dispatcher.UIThread.InvokeAsync(() =>
{
    // ...

    // Изменяем свойство модели, которое должно сразу же отразиться
    // в пользовательском интерфейсе
    viewModel.Message = "When you finish counting, press the button";

    // Активируем кнопку завершения пересчётов
    viewModel.IsButtonEnabled = true;

    // ...
});
```

При настройке сервера указывается Startup-класс, который добавляет middleware delegate, обрабатывающий http-запрос:

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            await context.Response.WriteAsync("Hello from Avalonia HTTP Server!");
        });
    }
}
```

В Startup-классе часто определены два метода:

- ConfigureServices(IServiceCollection services): в этом методе обычно регистрируются сервисы приложения, такие как: контекст базы данных, механизм аутентификации, и т.д. Регистрация осуществляется через контейнер Dependancy Injection
- Configure(IApplicationBuilder app, IWebHostEnvironment env): этот метод определяет как приложение отвечает на HTTP-запросы, а также позволяет настроить middleware
