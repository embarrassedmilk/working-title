namespace Snake.FRunner.Controllers
open WorkingTitle.Domain.Accounts
open WorkingTitle.Domain.Accounts.Commands
open Microsoft.AspNetCore.Mvc
open WorkingTitle.Utils.RResult
open WorkingTitle.Persistence.EventStore.Store

[<Route("api/[controller]")>]
type AccountController () =
    inherit Controller()

    let store = Store(StoreSettings("ConnectTo=tcp://admin:changeit@localhost:1113;"))

    let getEvents (id:string) =
        store.GetEvents id
        |> Async.RunSynchronously
        |>> Array.toList

    [<HttpPost>]
    [<Route("create")>]
    member __.CreateAccount([<FromBody>]cmd:CreateAccount) =
        let res = Account.Create cmd >>= (fun (evts, _) -> store.Store evts.Head.EntityId evts |> Async.RunSynchronously)

        match res with
        | RResult.Good _   -> __.Ok "OK"                             :> IActionResult
        | RResult.Bad  rbad -> (rbad.Describe() |> __.BadRequest)    :> IActionResult

    [<HttpPost>]
    [<Route("changeusername")>]
    member __.ChangeUsername([<FromBody>]cmd:ChangeAccountUsername) = 
        let res = getEvents cmd.Id
                |>> Account.ReplayAccount
                >>= Account.ChangeUsername cmd
                >>= (fun (evts, _) -> store.Store cmd.Id evts |> Async.RunSynchronously)

        match res with
        | RResult.Good _    -> __.Ok "Success!"                     :> IActionResult
        | RResult.Bad  rbad -> (rbad.Describe() |> __.BadRequest)   :> IActionResult

    [<HttpPost>]
    [<Route("changeemail")>]
    member __.ChangeEmail([<FromBody>]cmd:ChangeAccountEmail) = 
        let res = getEvents cmd.Id
                |>> Account.ReplayAccount
                >>= Account.ChangeEmail cmd
                >>= (fun (evts, _) -> store.Store cmd.Id evts |> Async.RunSynchronously)

        match res with
        | RResult.Good _    -> __.Ok "Success!"                     :> IActionResult
        | RResult.Bad  rbad -> (rbad.Describe() |> __.BadRequest)   :> IActionResult

    [<HttpGet("events/{id}")>]
    member __.GetEventsById(id:string) =
        let res = getEvents id
        
        match res with 
        | RResult.Good evts ->  __.Ok evts                        :> IActionResult
        | RResult.Bad  rbad -> (rbad.Describe() |> __.BadRequest) :> IActionResult

    [<HttpGet("state/{id}")>]
    member __.GetSnapshotById(id:string) =
        let res = getEvents id |>> Account.ReplayAccount
        
        match res with 
        | RResult.Good state  ->  __.Ok state                       :> IActionResult
        | RResult.Bad  rbad   -> (rbad.Describe() |> __.BadRequest) :> IActionResult