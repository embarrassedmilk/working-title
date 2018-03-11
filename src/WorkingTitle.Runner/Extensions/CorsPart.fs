namespace WorkingTitle.Runner.Extensions

open Snake.Core
open Microsoft.AspNetCore.Builder;
open Microsoft.AspNetCore.Hosting;
open Microsoft.Extensions.DependencyInjection;
open System.Collections.Generic

type CorsPart() = 
    interface IApplicationPlugin with
        member x.Configure(app:IApplicationBuilder, env:IHostingEnvironment) = 
            app.UseCors (fun pol -> 
                pol
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials() 
                |> ignore
            )
            |> ignore

        member x.ConfigureServices(services:IServiceCollection) =
            services.AddCors()
            |> ignore

        member x.BeforeBuild(app:IApplicationBuilder, env:IHostingEnvironment, services:IEnumerable<ServiceDescriptor>) = 
            ()