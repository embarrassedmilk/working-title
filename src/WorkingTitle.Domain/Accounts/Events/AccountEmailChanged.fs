namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Primitives

type AccountEmailChanged(id: Guid, email: Email) =
    member this.Email = email

    interface ICmsEvent with
        member this.EntityId = id
        member this.Timestamp = DateTimeOffset.UtcNow