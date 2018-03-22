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

    let getEvents (id:string) =
        store.GetEvents id
        |> Async.RunSynchronously
        |>> Array.toList

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
                >>= (fun (evts, _) -> store.Store cmd.EntityId evts |> Async.RunSynchronously)

        match res with
        | RResult.Good _    -> __.Ok "Success!"                     :> IActionResult
        | RResult.Bad  rbad -> (rbad.Describe() |> __.BadRequest)   :> IActionResult

    [<HttpGet("posts/{id}")>]
    member __.GetSnapshotById(id:string) =
        let res = getEvents id |>> Post.ReplayPost
        
        match res with 
        | RResult.Good state  ->  __.Ok state                       :> IActionResult
        | RResult.Bad  rbad   -> (rbad.Describe() |> __.BadRequest) :> IActionResult

// Orchestration part:
// event is saved to ES
// event gets published to Queue
// Redis picks up message from the Queue
// Applies event, saves it

// Background part:
// Redis is listenting to Rabbit