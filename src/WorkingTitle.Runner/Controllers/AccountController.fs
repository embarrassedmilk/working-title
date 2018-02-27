﻿namespace Snake.FRunner.Controllers
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

    let getEvents (id:Guid) =
        EventStore.Get id

    [<HttpPost>]
    [<Route("create")>]
    member __.CreateAccount([<FromBody>]cmd:CreateAccount) =
        let (evt, state) = Account.Create cmd
        EventStore.Store evt
        state

    [<HttpPost>]
    [<Route("changeusername")>]
    member __.ChangeUsername([<FromBody>]cmd:ChangeAccountUsername) = 
        let state = getEvents cmd.Id |>  Account.GetAccountStateFromEvents
        let (evt, newState) = Account.ChangeUsername state cmd
        EventStore.Store evt
        newState

    [<HttpPost>]
    [<Route("changeemail")>]
    member __.ChangeEmail([<FromBody>]cmd:ChangeAccountEmail) = 
        let state = getEvents cmd.Id |> Account.GetAccountStateFromEvents
        let (evt, newState) = Account.ChangeEmail state cmd
        EventStore.Store evt
        newState

    [<HttpGet("events/{id}")>]
    member __.GetEventsById(id:Guid) =
        getEvents id

    [<HttpGet("state/{id}")>]
    member __.GetSnapshotById(id:Guid) =
        getEvents id |> Account.GetAccountStateFromEvents