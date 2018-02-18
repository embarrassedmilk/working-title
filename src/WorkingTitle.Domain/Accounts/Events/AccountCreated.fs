namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Primitives

type AccountCreated(id: Guid, email: Email, username: Username) =
    member this.Email = email
    member this.Username = username

    interface ICmsEvent with
        member this.EntityId = id
        member this.Timestamp = DateTimeOffset.UtcNow