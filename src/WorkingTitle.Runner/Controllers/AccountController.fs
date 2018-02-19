namespace Snake.FRunner.Controllers
open System
open WorkingTitle.Domain.Accounts
open WorkingTitle.Domain.Accounts.Snapshots
open WorkingTitle.Domain.Accounts.Commands
open WorkingTitle.Persistence.InMemory
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Builder

[<Route("api/[controller]")>]
type AccountController () =
    inherit Controller()

    [<HttpPost>]
    member __.CreateAccount([<FromBody>]cmd:CreateAccount) =
        let (evt, state) = Account.Create cmd
        EventStore.Store evt
        state

    [<HttpPost>]
    member __.ChangeUsername([<FromBody>]cmd:ChangeAccountUsername) = 
        let state = EventStore.Get cmd.Id |> Account.GetAccountStateFromEvents
        let (evt, newState) = Account.ChangeUsername state cmd
        EventStore.Store evt
        newState

    [<HttpGet("{id}")>]
    member __.GetEventsById(id:Guid) =
        EventStore.Get id

    [<HttpGet("{id}")>]
    member __.GetSnapshotById(id:Guid) =
        EventStore.Get id |> Account.GetAccountStateFromEvents