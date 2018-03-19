namespace WorkingTitle.Runner.Extensions

open Snake.Core
open WorkingTitle.Runner.Hubs
open Microsoft.AspNetCore.Builder;
open Microsoft.AspNetCore.Hosting;
open Microsoft.Extensions.DependencyInjection;
open System.Collections.Generic
open WorkingTitle.Runner


type BackgroundPart(settings: Settings) = 
    member this.Settings = settings

    interface IApplicationPlugin with
        member x.Configure(app:IApplicationBuilder, env:IHostingEnvironment) = 
            ()

        member x.ConfigureServices(services:IServiceCollection) =
            ()

        member x.BeforeBuild(app:IApplicationBuilder, env:IHostingEnvironment, services:IEnumerable<ServiceDescriptor>) = 
            ()