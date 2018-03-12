namespace Snake.FRunner.Controllers
open Microsoft.AspNetCore.Mvc
open WorkingTitle.Persistence.EventStore.Store
open Microsoft.AspNetCore.SignalR
open WorkingTitle.Runner.Hubs
open WorkingTitle.Domain.Posts.Commands
open WorkingTitle.Utils.RResult
open WorkingTitle.Domain.Posts

[<Route("api/[controller]")>]
type PostsController (hc:IHubContext<EventsHub>) =
    inherit Controller()

    let store = Store(StoreSettings("ConnectTo=tcp://admin:changeit@localhost:1113;"))

    let getEvents (id:string) =
        store.GetEvents id
        |> Async.RunSynchronously
        |>> Array.toList

    [<HttpPost>]
    [<Route("publish")>]
    member __.PublishPost([<FromBody>]cmd:PublishPost) =
        let res = Post.Publish cmd >>= (fun (evts, _) -> store.Store evts.Head.EntityId evts |> Async.RunSynchronously)

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