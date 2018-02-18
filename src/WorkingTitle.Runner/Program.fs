open Snake.Core
open Snake.Extensions.Mvc
open Snake.Extensions.Swagger
open Snake.Extensions.Serilog
open Microsoft.AspNetCore.Hosting
open Serilog

let getLoggerConfiguration = 
    LoggerConfiguration()
        .Enrich
        .FromLogContext()
        .WriteTo.Console()

[<EntryPoint>]
let main argv =
    SnakeWebHostBuilder<BaseSettings>
        .CreateDefaultBuilder(argv)
        .WithMvc()
        .WithSwagger("ES app")
        .WithSerilog(getLoggerConfiguration)
        .Build("appsettings.json", "WorkingTitle.Runner")
        .Run()
    0