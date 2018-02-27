namespace WorkingTitle.Domain.Accounts.Commands

open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Primitives
open WorkingTitle.Domain.Accounts.Events
open System

type CreateAccount(email: string, username: string) =
    member this.Email = email
    member this.Username = username
    member this.ToEvent() = 
        let email = EmailAddress.create this.Email
        let created = AccountCreated(Guid.NewGuid(), DateTimeOffset.UtcNow, , this.Username)
        Created created