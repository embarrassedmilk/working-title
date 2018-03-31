namespace Snake.FRunner.Controllers
open Microsoft.AspNetCore.Mvc
open WorkingTitle.Persistence.EventStore.Store
open Microsoft.AspNetCore.SignalR
open WorkingTitle.Runner.Hubs
open WorkingTitle.Domain.Posts.Commands
open WorkingTitle.Utils.RResult
open WorkingTitle.Domain.Posts
open WorkingTitle.RabbitMQ
open WorkingTitle.Runner
open WorkingTitle.Redis

[<Route("api/[controller]")>]
type PostsController (hc:IHubContext<EventsHub>, settings: Settings) =
    inherit Controller()

    let store = Store(StoreSettings(settings.EventSourceConnectionString))
    let rabbit = MessageQueueWriter(
                    WorkingTitle.RabbitMQ.MessageQueueSettings(
                        settings.MessageQueueSettings.ConnectionString, 
                        settings.MessageQueueSettings.QueueName
                    )
                 )

    let cache = Cache(CacheSettings(settings.RedisConnectionString))

    let getEvents (id:string) =
        store.GetEvents id
        |> Async.RunSynchronously

    let getSnapshot (id:string) =
        cache.GetPostById id
        |> Async.RunSynchronously

    [<HttpPost>]
    [<Route("publish")>]
    member __.PublishPost([<FromBody>]cmd:PublishPost) =
        let res = 
            Post.Publish cmd 
                >>= (fun (evts, _) -> 
                    store.Store evts.Head.EntityId evts |> Async.RunSynchronously 
                        |- (fun (_) -> evts |> List.iter (rabbit.Publish >> ignore)))

        match res with
        | RResult.Good _    -> __.Ok "OK"                               :> IActionResult
        | RResult.Bad  rbad -> (rbad.Describe() |> __.BadRequest)       :> IActionResult

    [<HttpPut>]
    [<Route("edit")>]
    member __.EditPost([<FromBody>]cmd:ChangePostContent) = 
        let res = getEvents cmd.EntityId
                |>> Post.ReplayPost
                >>= Post.Edit cmd
                >>= (fun (evts, _) -> 
                    store.Store cmd.EntityId evts |> Async.RunSynchronously 
                        |- (fun (_) -> evts |> List.iter (rabbit.Publish >> ignore)))

        match res with
        | RResult.Good _    -> __.Ok "Success!"                     :> IActionResult
        | RResult.Bad  rbad -> (rbad.Describe() |> __.BadRequest)   :> IActionResult

    [<HttpGet("posts/{id}")>]
    member __.GetSnapshotById(id:string) =
        match (getSnapshot id) with 
        | RResult.Good state  ->  __.Ok state.Value                 :> IActionResult
        | RResult.Bad  rbad   -> (rbad.Describe() |> __.BadRequest) :> IActionResult

    [<HttpPut("posts/rehydrate")>]
    member __.Rehydrate() =
        let res = cache.Rehydrate(store) |> Async.RunSynchronously

        match res with 
        | RResult.Good _      ->  __.Ok()                           :> IActionResult
        | RResult.Bad  rbad   -> (rbad.Describe() |> __.BadRequest) :> IActionResult