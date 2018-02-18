namespace WorkingTitle.Domain.Accounts.Commands

open WorkingTitle.Domain.Primitives
open WorkingTitle.Domain.Accounts.Events
open System

type ChangeAccountEmail(id: Guid, email: Email) =
    member this.Id = id
    member this.ToEvent() = 
        AccountEmailChanged(id, DateTimeOffset.UtcNow, email)