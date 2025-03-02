using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Windows;
using TestTask.Service.Cients.Implementation;
using TestTask.Service.Cients.Interfaces;
using TestTask.Service.Connector.Implementation;
using ConnectorTest;
using TestTask.GUI_Framework__WPF_.ViewModel;
using TestTask.Service.HandleProcessing.Interface;
using TestTask.Service.HandleProcessing.Implementation;
namespace TestTask.GUI_Framework__WPF_
{

    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainWindow>();

                    services.AddHttpClient();
                    services.AddSingleton<IRestClient>(provider =>
                    {
                        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                        return new RestClient(httpClientFactory);
                    });
                    services.AddSingleton<IHandler, Handler>();
                    services.AddSingleton<MainWindowViewModel>();

                    services.AddTransient<ITestConnector,Connector>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }

}
