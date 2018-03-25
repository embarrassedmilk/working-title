namespace WorkingTitle.Domain.Posts.Commands

open System
open WorkingTitle.Utils.RResult
open WorkingTitle.Domain.Posts.Events
open WorkingTitle.Domain.EventSource

type ChangePostContent(entityId: string, content: string) = 
    member this.EntityId = entityId
    member this.Timestamp = DateTimeOffset.UtcNow
    member this.Content = content

    member this.ToEvent() = 
        RResult.rgood (PostContentChanged(this.EntityId, DateTimeOffset.UtcNow, this.Content))
        |>> Edited