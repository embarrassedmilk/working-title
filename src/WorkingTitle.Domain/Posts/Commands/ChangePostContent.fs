namespace WorkingTitle.Domain.Posts.Commands

open System
open WorkingTitle.Utils.RResult
open WorkingTitle.Domain.Posts.Events
open WorkingTitle.Domain.EventSource

type ChangePostContent(id: string, timestamp: DateTimeOffset, content: string) = 
    member this.EntityId = id
    member this.Timestamp = timestamp
    member this.Content = content

    member this.ToEvent() = 
        RResult.rgood (PostContentChanged(this.EntityId, DateTimeOffset.UtcNow, this.Content))
        |>> Edited