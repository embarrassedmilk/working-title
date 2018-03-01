namespace WorkingTitle.Domain.Accounts.Events

open System
open WorkingTitle.Domain.Primitives

type AccountUsernameChanged(id: Guid, timestamp: DateTimeOffset, username: string) =
    member this.Username = username
    member this.EntityId = id
    member this.Timestamp = timestamp