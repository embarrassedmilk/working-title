namespace WorkingTitle.Domain.Accounts.Commands

open WorkingTitle.Domain.Primitives
open WorkingTitle.Domain.Accounts.Events
open System

type ChangeAccountUsername(id: Guid, username: Username) =
    member this.Id = id
    member this.ToEvent() = 
        AccountUsernameChanged(id, DateTimeOffset.UtcNow, username)