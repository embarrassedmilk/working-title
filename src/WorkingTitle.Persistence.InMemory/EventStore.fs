namespace WorkingTitle.Persistence.InMemory

open WorkingTitle.Domain.EventSource
open System

type EventStore() =
    static let mutable _eventList = []

    static member Store(evt: ICmsEvent) = 
        _eventList <- evt::_eventList

    static member Get(entityId: Guid) =
        _eventList |> List.filter(fun evt -> Guid.Equals(evt.EntityId, entityId))