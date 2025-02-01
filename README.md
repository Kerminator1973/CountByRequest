# CountByRequest

Experimental hybrid: AspNetCore + Avalonia in one app



## Запуск простейшего web-сервера, который имитирует банковскую систему

В папке "serverExpress" находится исходный код на Node.js, который имитирует банковский сервер. Код приложения приведён в файле "server.js". Чтобы запустить сервер необходимо установить Node.js, скачать зависимости и запусить сервер.

Скачать Node.js можно с [официального сайта](https://nodejs.org/en) проекта. Текущая LTS-версия 22.13.1. Следует заметить, что эта сборка не работает на Windows 7 - необходимо использовать Windows 10/11, или Linux.


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
