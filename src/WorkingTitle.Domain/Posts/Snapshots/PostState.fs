namespace WorkingTitle.Domain.Posts.Snapshots

open System

type ExistingPostState(entityId: string, timestamp: DateTimeOffset, author: string, content: string) = 
    member this.EntityId = entityId
    member this.Author = author
    member this.Timestamp = timestamp
    member this.Content = content

    member this.WithContent (content: string) =
        ExistingPostState(this.EntityId, this.Timestamp, this.Author, content)


type PostState = 
    | PublishedPost of ExistingPostState 
    | NonExistingPost
