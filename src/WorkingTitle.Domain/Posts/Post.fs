namespace WorkingTitle.Domain.Posts

open WorkingTitle.Domain.Posts.Snapshots
open WorkingTitle.Domain.Posts.Commands
open WorkingTitle.Domain.EventSource

type Post() = 
    static member Apply (s: PostState) (evt: Events) = 
        match s, evt with
        | NonExistingPost _, Published published ->
            let newState = ExistingPostState(published.EntityId, published.Timestamp, published.Author, published.Content)
            PublishedPost newState
        | PublishedPost state, Edited edited ->
            let newState = state.WithContent(edited.Content)
            PublishedPost newState
        | _,_ ->
            s

    static member Publish (cmd:PublishPost) = 
        cmd.ToEvent()
        |>> (fun evt -> 
            let state = Post.Apply NonExistingPost evt
            ([evt], state))

    static member Edit (cmd:ChangePostContent) (currentState:PostState) =
        cmd.ToEvent()
        |>> (fun evt -> 
            let state = Post.Apply currentState evt
            ([evt], state))

    static member ReplayPost (evts:Events list) =
        evts |> List.fold Post.Apply NonExistingPost