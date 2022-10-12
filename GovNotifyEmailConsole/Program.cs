using GovNotifyEmailConsole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Interfaces;

//https://stackoverflow.com/questions/71954271/how-can-i-read-the-appsettings-json-in-a-net-6-console-application
// https://bjdejongblog.nl/net-core-6-dependency-injection-console-program/
var host = CreateHostBuilder(args).Build();
var app = host.Services.GetRequiredService<Application>();

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices(
            (config, services) =>
            {
                var govNotifyApiKey = config.Configuration["EmailSettings:GovNotifyApiKey"];
                services
                    .AddSingleton<Application, Application>()
                    .AddSingleton<IEmailService, EmailService>();
                services.AddTransient<IAsyncNotificationClient, NotificationClient>(
                    _ => new NotificationClient(govNotifyApiKey));

            });
}


class Application
{
    EmailSettings _emailSettings;
    private IAsyncNotificationClient _notificationClient;

    public Application(IConfiguration configuration,
        IAsyncNotificationClient notificationClient,
        ILogger<Application> logger)
    {
        _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        _notificationClient = notificationClient;

        Console.WriteLine("1. temporary-failure");
        Console.WriteLine("2. permanent-failure");
        //Console.WriteLine("3. normal");
        Console.WriteLine("Any other key to exit.");

        var key = Console.ReadKey();
        var recipient = key.Key switch
        {
            ConsoleKey.D1 or ConsoleKey.NumPad1 => "emp-fail@simulator.notify",
            ConsoleKey.D2 or ConsoleKey.NumPad2 => "perm-fail@simulator.notify",
            ConsoleKey.D3 or ConsoleKey.NumPad3 => _emailSettings.SupportEmailAddress,
            _ => default
        };

        if (string.IsNullOrWhiteSpace(recipient)) return;

        logger.LogInformation("Preparing to send to {target}", recipient);
        
        var result = _notificationClient
            .SendEmailAsync(recipient, _emailSettings.TestTemplateId)
            .GetAwaiter().GetResult();

        logger.LogInformation("Result: {id} {reference} {uri}", result.id, result.reference, result.uri);
        logger.LogInformation("Result: {content}", result.content);
    }
}