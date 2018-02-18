namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Primitives

type AccountCreated(id: Guid, timestamp: DateTimeOffset, email: Email, username: Username) =
    member this.Email = email
    member this.Username = username
    member this.EntityId = (this :> ICmsEvent).EntityId
    member this.Timestamp = (this :> ICmsEvent).Timestamp

    interface ICmsEvent with
        member this.EntityId = id
        member this.Timestamp = timestamp