using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WindowsService;

IHost host = Host.CreateDefaultBuilder()
    .UseWindowsService(configure =>
    {
        configure.ServiceName = "Tactical Windows Services";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<CleanupFolderService>();
    })
    .Build();

host.Run();
