namespace Snake.FRunner.Controllers
open System
open WorkingTitle.Domain.Accounts
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
        EventWriter.Store evt
        state

    [<HttpGet("{id}")>]
    member __.GetEventsById(id:Guid) =
        EventWriter.Get id