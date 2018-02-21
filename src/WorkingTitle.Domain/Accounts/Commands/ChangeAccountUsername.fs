namespace WorkingTitle.Domain.Accounts.Commands

open WorkingTitle.Domain.Primitives
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Accounts.Events
open System
open System

type ChangeAccountUsername(id: Guid, username: Username) =
    member this.Id = id
    member this.ToEvent() = 
        let usernameChanged = AccountUsernameChanged(id, DateTimeOffset.UtcNow, username)
        (UsernameChanged usernameChanged, usernameChanged.EntityId)