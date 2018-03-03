namespace WorkingTitle.Domain.Accounts.Commands

open WorkingTitle.Domain.Primitives
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Accounts.Events
open System

type ChangeAccountEmail(id: string, email: string) =
    member this.Id = id
    member this.Email = email
    member this.ToEvent() = 
        EmailAddress.create this.Email
        |>> (fun email -> AccountEmailChanged(id, DateTimeOffset.UtcNow, email))
        |>> EmailChanged