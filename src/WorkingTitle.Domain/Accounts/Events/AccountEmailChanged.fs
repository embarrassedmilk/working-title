namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.Primitives

type AccountEmailChanged(id: Guid, timestamp: DateTimeOffset, email: string) =
    member this.Email = email   
    member this.EntityId = id
    member this.Timestamp = timestamp