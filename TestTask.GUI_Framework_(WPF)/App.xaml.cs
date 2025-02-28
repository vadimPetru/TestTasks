using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Windows;
using TestTask.Service.Cients.Implementation;
using TestTask.Service.Cients.Interfaces;
using TestTask.Service.Connector.Implementation;
using ConnectorTest;
using TestTask.GUI_Framework__WPF_.ViewModel;
using TestTask.GUI_Framework__WPF_.ViewModel.Modal;
using TestTask.GUI_Framework__WPF_.View.Windows;
using TestTask.GUI_Framework__WPF_.View.Modal;
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
                    services.AddSingleton<ModelWindow>();
                    services.AddSingleton<CandleContent>();
                    services.AddSingleton<TradeContent>();

                    services.AddHttpClient();
                    services.AddSingleton<IRestClient>(provider =>
                    {
                        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                        return new RestClient(httpClientFactory);
                    });

                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<ModelWindowViewModel>();
                    services.AddSingleton<CandleContentViewModel>();
                    services.AddSingleton<TradeContentViewModel>();

                    services.AddSingleton<ITestConnector,Connector>();
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
