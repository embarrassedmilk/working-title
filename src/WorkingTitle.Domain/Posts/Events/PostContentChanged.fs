namespace WorkingTitle.Domain.Posts.Events

open System

type PostContentChanged(entityId: string, timestamp: DateTimeOffset, content: string) =
    member this.EntityId = entityId
    member this.Timestamp = timestamp
    member this.Content = content