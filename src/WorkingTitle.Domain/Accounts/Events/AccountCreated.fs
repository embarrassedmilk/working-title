namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.Primitives

type AccountCreated(id: Guid, timestamp: DateTimeOffset, email: string, username: string) =
    member this.Email = email
    member this.Username = username
    member this.EntityId = id
    member this.Timestamp = timestamp