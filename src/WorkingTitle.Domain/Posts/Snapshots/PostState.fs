namespace WorkingTitle.Domain.Posts.Snapshots

open System

type ExistingPostState(id: string, timestamp: DateTimeOffset, author: string, content: string) = 
    member this.EntityId = id
    member this.Author = author
    member this.Timestamp = timestamp
    member this.Content = content

    member this.WithContent (content: string) =
        ExistingPostState(this.EntityId, this.Timestamp, this.Author, content)


type PostState = 
    | PublishedPost of ExistingPostState 
    | NonExistingPost
