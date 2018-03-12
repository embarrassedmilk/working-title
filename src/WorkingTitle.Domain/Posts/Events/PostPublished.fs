namespace WorkingTitle.Domain.Posts.Events

open System

type PostPublished(entityId: string, timestamp: DateTimeOffset, author: string, content: string) =
    member this.EntityId = entityId
    member this.Timestamp = timestamp
    member this.Author = author
    member this.Content = content