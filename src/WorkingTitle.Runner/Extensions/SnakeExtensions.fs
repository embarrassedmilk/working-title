namespace WorkingTitle.Runner.Extensions

open Snake.Core
open WorkingTitle.Runner.Hubs
open Microsoft.AspNetCore.Builder;
open Microsoft.AspNetCore.Hosting;
open Microsoft.Extensions.DependencyInjection;
open System.Collections.Generic


type SignalRPart() = 
    interface IApplicationPlugin with
        member x.Configure(app:IApplicationBuilder, env:IHostingEnvironment) = 
            app.UseSignalR(fun routes -> routes.MapHub<EventsHub>("events"))
            |> ignore

        member x.ConfigureServices(services:IServiceCollection) =
            services.AddSignalR()
            |> ignore

        member x.BeforeBuild(app:IApplicationBuilder, env:IHostingEnvironment, services:IEnumerable<ServiceDescriptor>) = 
            ()


type CorsPart() = 
    interface IApplicationPlugin with
        member x.Configure(app:IApplicationBuilder, env:IHostingEnvironment) = 
            app.UseCors (fun pol -> pol.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() |> ignore)
            |> ignore

        member x.ConfigureServices(services:IServiceCollection) =
            services.AddCors()
            |> ignore

        member x.BeforeBuild(app:IApplicationBuilder, env:IHostingEnvironment, services:IEnumerable<ServiceDescriptor>) = 
            ()