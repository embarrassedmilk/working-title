namespace WorkingTitle.Domain.Accounts.Commands

open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Primitives
open WorkingTitle.Domain.Accounts.Events
open System

type CreateAccount(email: Email, username: Username) =
    member this.Email = email
    member this.Username = username
    member this.ToEvent() = 
        let created = AccountCreated(Guid.NewGuid(), DateTimeOffset.UtcNow, email, username)
        (Created created, created.EntityId)