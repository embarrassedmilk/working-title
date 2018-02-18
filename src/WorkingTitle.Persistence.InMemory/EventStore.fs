namespace WorkingTitle.Persistence.InMemory

open WorkingTitle.Domain.EventSource
open System

type EventWriter() =
    static let mutable _eventList = []

    static member Store(evt: ICmsEvent) = 
        _eventList <- evt::_eventList

    static member Get(entityId: Guid) =
        let entityEvt (evt: ICmsEvent) = Guid.Equals(evt.EntityId, entityId)
        List.filter entityEvt _eventList