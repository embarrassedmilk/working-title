namespace WorkingTitle.Persistence.InMemory

open WorkingTitle.Domain.Infrastructure
open WorkingTitle.Domain.EventSource
open System

type EventWriter() =
    let mutable _eventList = []

    interface IEventWriter with
        member this.Store(evt: ICmsEvent) = 
            _eventList <- evt::_eventList

    interface IEventReader with
        member this.Get(entityId: Guid) =
            _eventList 
            |> List.filter(fun evt -> Guid.Equals(evt.EntityId, entityId))