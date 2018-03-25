namespace WorkingTitle.Domain.EventSource

open System
open WorkingTitle.Domain.Posts.Events

type Events =
    | Published of PostPublished
    | Edited of PostContentChanged
    member x.EntityId : string = 
        match x with 
        | Published published -> published.EntityId
        | Edited edited -> edited.EntityId

    member x.Timestamp : DateTimeOffset = 
        match x with 
        | Published published -> published.Timestamp
        | Edited edited -> edited.Timestamp