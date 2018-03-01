namespace Snake.FRunner.Controllers
open System
open WorkingTitle.Domain.Accounts
open WorkingTitle.Domain.Accounts.Commands
open WorkingTitle.Persistence.InMemory
open Microsoft.AspNetCore.Mvc
open WorkingTitle.Utils.RResult

[<Route("api/[controller]")>]
type AccountController () =
    inherit Controller()

    let getEvents (id:Guid) =
        EventStore.Get id

    [<HttpPost>]
    [<Route("create")>]
    member __.CreateAccount([<FromBody>]cmd:CreateAccount) =
        match Account.Create cmd with
        | RResult.Good (evt, state) -> EventStore.Store evt ; __.Ok state :> IActionResult
        | RResult.Bad  rbad         -> (rbad.Describe() |> __.BadRequest) :> IActionResult

    [<HttpPost>]
    [<Route("changeusername")>]
    member __.ChangeUsername([<FromBody>]cmd:ChangeAccountUsername) = 
        let state = getEvents cmd.Id |>  Account.GetAccountStateFromEvents
        match Account.ChangeUsername state cmd with
        | RResult.Good (evt, newState)  -> EventStore.Store evt ; __.Ok newState    :> IActionResult
        | RResult.Bad  rbad             -> (rbad.Describe() |> __.BadRequest)       :> IActionResult

    [<HttpPost>]
    [<Route("changeemail")>]
    member __.ChangeEmail([<FromBody>]cmd:ChangeAccountEmail) = 
        let state = getEvents cmd.Id |> Account.GetAccountStateFromEvents
        match Account.ChangeEmail state cmd with
        | RResult.Good (evt, newState)  -> EventStore.Store evt ; __.Ok newState    :> IActionResult
        | RResult.Bad  rbad             -> (rbad.Describe() |> __.BadRequest)       :> IActionResult

    [<HttpGet("events/{id}")>]
    member __.GetEventsById(id:Guid) =
        getEvents id

    [<HttpGet("state/{id}")>]
    member __.GetSnapshotById(id:Guid) =
        getEvents id |> Account.GetAccountStateFromEvents