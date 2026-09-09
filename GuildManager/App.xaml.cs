using System;
using System.Configuration;
using System.Data;
using System.Windows;
using GuildManager.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MonProjet;


public partial class App : Application
{
    private WebApplication? _apiApp;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.Services.AddControllers();
        builder.Services.AddInfrastructure(builder.Configuration);

        _apiApp = builder.Build();
        _apiApp.MapControllers();

        _ = _apiApp.RunAsync("http://0.0.0.0:5080");
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_apiApp is not null)
            await _apiApp.StopAsync();

        base.OnExit(e);
    }
}