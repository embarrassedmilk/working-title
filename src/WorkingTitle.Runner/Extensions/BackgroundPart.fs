namespace WorkingTitle.Runner.Extensions

open Snake.Core
open WorkingTitle.Runner.Hubs
open Microsoft.AspNetCore.Builder;
open Microsoft.AspNetCore.Hosting;
open Microsoft.Extensions.DependencyInjection;
open System.Collections.Generic
open WorkingTitle.Runner
open WorkingTitle.RabbitMQ
open WorkingTitle.Redis
open FluentScheduler

type BackgroundPart(settings: Settings) = 
    member this.Settings = settings

    interface IApplicationPlugin with
        member x.Configure(app:IApplicationBuilder, env:IHostingEnvironment) = 
            ()

        member x.ConfigureServices(services:IServiceCollection) =
            services.AddSingleton<Settings>(x.Settings) |> ignore
            ()

        member x.BeforeBuild(app:IApplicationBuilder, env:IHostingEnvironment, services:IEnumerable<ServiceDescriptor>) = 
            let rabbit = 
                    MessageQueueReader(
                        WorkingTitle.RabbitMQ.MessageQueueSettings(
                            settings.MessageQueueSettings.ConnectionString, 
                            settings.MessageQueueSettings.QueueName
                        )
                     )
            let cache = Cache(CacheSettings(settings.RedisConnectionString))
            JobManager.AddJob((fun (_) -> cache.CheckQueue(rabbit) |> Async.RunSynchronously |> ignore), (fun (s) -> s.NonReentrant().ToRunEvery(5).Seconds() |> ignore))
            ()