namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Primitives

type AccountUsernameChanged(id: Guid, username: Username) =
    member this.Username = username

    interface ICmsEvent with
        member this.EntityId = id
        member this.Timestamp = DateTimeOffset.UtcNow