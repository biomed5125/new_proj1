using HMS.CommBench.Service;
using HMS.CommBench.ViewModels;
using HMS.CommBench.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;

namespace HMS.CommBench
{
    public partial class App : Application
    {
        private ServiceProvider? _sp;

        protected override void OnStartup(StartupEventArgs e)
        {
            // read appsettings.json → Api:BaseUrl
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var baseUrl = cfg["Api:BaseUrl"] ?? "https://localhost:7190";
            Debug.WriteLine($"[CommBench] BaseUrl = {baseUrl}");

            var services = new ServiceCollection();

            // ONE handler/client that accepts the dev cert and uses HTTPS base URL
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var http = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
            services.AddSingleton(http);
            services.AddSingleton(new CommApiClient(http));
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            _sp = services.BuildServiceProvider();
            _sp.GetRequiredService<MainWindow>().Show();
        }
    }
}
