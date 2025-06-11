using Ninject;
using PokeCore.Hosting.Modules;
using PokeLab.Application.ContentBrowser;
using PokeLab.Infrastructure.ContentBrowser;

namespace PokeLab.Infrastructure;

public sealed class PokeLabInfrastructureModule : Module
{
    public override string Name => "PokéLab Infrastructures";

    public override void ConfigureServices(IKernel kernel)
    {
        kernel.Bind<IContentBrowserCache>().To<DefaultContentBrowserCache>();
        kernel.Bind<IContentBrowserNavigator>().To<DefaultContentBrowserNavigator>();
    }
}