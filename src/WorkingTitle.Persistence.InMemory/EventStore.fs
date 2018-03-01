namespace WorkingTitle.Persistence.InMemory

open System
open WorkingTitle.Domain.EventSource

type EventStore() =
    static let mutable _eventList = []

    static member Store(evt: Events) = 
        _eventList <- evt::_eventList

    static member Get(entityId: Guid) =
        _eventList |> List.rev |> List.filter(fun evt -> Guid.Equals(evt.EntityId, entityId))