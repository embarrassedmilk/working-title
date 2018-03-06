open Snake.Core
open Snake.Extensions.Mvc
open Snake.Extensions.Swagger
open Snake.Extensions.Serilog
open Microsoft.AspNetCore.Hosting
open Serilog
open WorkingTitle.Runner
open WorkingTitle.Runner.Extensions

let getLoggerConfiguration = 
    LoggerConfiguration()
        .Enrich
        .FromLogContext()
        .WriteTo.Console()

[<EntryPoint>]
let main argv =
    SnakeWebHostBuilder<Settings>
        .CreateDefaultBuilder(argv)
        .With(fun () -> CorsPart() :> IApplicationPlugin)
        .With(fun () -> SignalRPart() :> IApplicationPlugin)
        .With(fun () -> AuthenticationPart() :> IApplicationPlugin)
        .WithMvc()
        .WithSwagger("ES app")
        .WithSerilog(getLoggerConfiguration)
        .Build("appsettings.json", "WorkingTitle.Runner")
        .Run()
    0