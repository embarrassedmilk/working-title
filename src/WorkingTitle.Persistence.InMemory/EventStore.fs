namespace WorkingTitle.Persistence.InMemory

open System
open WorkingTitle.Domain.EventSource

type Envelope = (Guid*AccountEvents) 

type EventStore() =
    static let mutable _eventList = []

    static member Store(evt: Envelope) = 
        _eventList <- evt::_eventList

    static member Get(entityId: Guid) =
        _eventList |> List.rev |> List.filter(fun evt -> Guid.Equals((fst evt), entityId))