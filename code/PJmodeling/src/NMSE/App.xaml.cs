using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NMSE.Contracts.Services;
using NMSE.Contracts.Views;
using NMSE.Core.Contracts.Services;
using NMSE.Core.Services;
using NMSE.Models;
using NMSE.Services;
using NMSE.ViewModels;
using NMSE.Views;
using Serilog;

namespace NMSE;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : System.Windows.Application
{
    private IHost _host;

    public T GetService<T>()
        where T : class
        => _host.Services.GetService(typeof(T)) as T;

    public App()
    {
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        //var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        var appLocation = System.AppContext.BaseDirectory;

    // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
    _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation);
                })
                .ConfigureServices(ConfigureServices)
                .Build();

        await _host.StartAsync();



    var AppSetup = SimProb.Instance;



    Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Debug()
      .WriteTo.File(AppSetup.sDebugLogFilePath, rollingInterval: RollingInterval.Day)
      .CreateLogger();

    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // TODO: Register your services, viewmodels and pages here

        // App Host
        services.AddHostedService<ApplicationHostService>();

        // Activation Handlers

        // Core Services
        services.AddSingleton<IFileService, FileService>();

        // Services
        services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();

    // Views and ViewModels

        services.AddTransient<SetupCheckViewModel>();
        services.AddTransient<SetupCheckPage>();

        services.AddTransient<IShellWindow, ShellWindow>();
        services.AddTransient<ShellViewModel>();

        services.AddTransient<SelectModelViewModel>();
        services.AddTransient<SelectModelPage>();

        services.AddTransient<ProjectDescriptionViewModel>();
        services.AddTransient<ProjectDescriptionPage>();

        services.AddTransient<Data1ViewModel>();
        services.AddTransient<Data1Page>();

        services.AddTransient<Data2ViewModel>();
        services.AddTransient<Data2Page>();

        services.AddTransient<Data3ViewModel>();
        services.AddTransient<Data3Page>();

        services.AddTransient<SimulationPropertiesViewModel>();
        services.AddTransient<SimulationPropertiesPage>();

        services.AddTransient<InterventionsViewModel>();
        services.AddTransient<InterventionsPage>();

        services.AddTransient<OutputSettingsViewModel>();
        services.AddTransient<OutputSettingsPage>();

        services.AddTransient<RunSimulationViewModel>();
        services.AddTransient<RunSimulationPage>();

        services.AddTransient<SimulationResultsViewModel>();
        services.AddTransient<SimulationResultsPage>();

        services.AddTransient<UserManualViewModel>();
        services.AddTransient<UserManualPage>();

        services.AddTransient<AboutViewModel>();
        services.AddTransient<AboutPage>();

        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsPage>();





        // Configuration
        services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        _host = null;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
