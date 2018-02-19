namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Primitives

type AccountEmailChanged(id: Guid, timestamp: DateTimeOffset, email: Email) =
    member this.Email = email   
    member this.EntityId = (this :> ICmsEvent).EntityId
    member this.Timestamp = (this :> ICmsEvent).Timestamp

    interface ICmsEvent with
        member this.EntityId = id
        member this.Timestamp = timestamp