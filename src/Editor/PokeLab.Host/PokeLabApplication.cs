using PokeCore.DependencyInjection.Abstractions;
using PokeCore.Hosting;
using PokeCore.Logging.Extensions;
using PokeLab.Application.Extensions;
using PokeLab.Infrastructure.Extensions;
using PokeLab.Presentation.Extensions;
using PokeLab.Presentation.ImGui.Extensions;

namespace PokeLab.Host;

public sealed class PokeLabApplication : App
{
    public override string AppName => "PokéSharp Editor";
    public override Version AppVersion => new(1, 0, 0);

    protected override void Configure(IServiceContainer services)
    {
        services.ConfigurePokeLabPresentation();
    }

    protected override void ConfigureServices(IServiceCollections services)
    {
        services.AddPokeLabApplication();
        services.AddPokeLabInfrastructure();
        services.AddPokeLabPresentation();
        services.AddPokeLabPresentationImGui();

        services.ConfigureLogging(x =>
        {
            x.AddConsoleLog();
            x.AddFileLog("logs");

            x.UseContextLogger();
        });
    }
}