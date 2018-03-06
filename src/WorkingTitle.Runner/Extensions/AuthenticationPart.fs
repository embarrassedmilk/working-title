namespace WorkingTitle.Runner.Extensions

open Snake.Core
open Microsoft.AspNetCore.Builder;
open Microsoft.AspNetCore.Hosting;
open Microsoft.Extensions.DependencyInjection;
open Microsoft.AspNetCore.Authentication.Google;
open System.Collections.Generic

type AuthenticationPart() = 
    interface IApplicationPlugin with
        member x.Configure(app:IApplicationBuilder, env:IHostingEnvironment) = 
            app.UseAuthentication()
            |> ignore

        member x.ConfigureServices(services:IServiceCollection) =
            services
                .AddAuthentication(fun options ->
                    options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme |> ignore
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme |> ignore
                )
                .AddGoogle(fun googleOptions ->  
                    googleOptions.ClientId = "" |> ignore
                    googleOptions.ClientSecret = "" |> ignore
                )
            |> ignore

        member x.BeforeBuild(app:IApplicationBuilder, env:IHostingEnvironment, services:IEnumerable<ServiceDescriptor>) = 
            ()