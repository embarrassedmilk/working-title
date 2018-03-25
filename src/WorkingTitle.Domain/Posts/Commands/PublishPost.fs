namespace WorkingTitle.Domain.Posts.Commands

open System
open WorkingTitle.Utils.RResult
open WorkingTitle.Domain.Posts.Events
open WorkingTitle.Domain.EventSource

type PublishPost(author: string, content: string) = 
    member this.Author = author
    member this.Timestamp = DateTimeOffset.UtcNow
    member this.Content = content
    
    member this.ToEvent() = 
        RResult.rgood (PostPublished(Guid.NewGuid().ToString(), DateTimeOffset.UtcNow, this.Author, this.Content))
        |>> Published