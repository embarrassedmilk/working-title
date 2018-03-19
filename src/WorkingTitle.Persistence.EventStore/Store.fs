namespace WorkingTitle.Persistence.EventStore

open System
open WorkingTitle.Domain.EventSource

open EventStore.ClientAPI
open Newtonsoft.Json
open WorkingTitle.Utils.RResult

module Store =
    type StoreSettings (connectionString: string) =
        member x.ConnectionString = connectionString

    type Store(settings: StoreSettings) =
        let getDataArray (evt: Events) =
            JsonConvert.SerializeObject evt 
            |> System.Text.Encoding.ASCII.GetBytes

        let getEvent (evt: byte[]) =
            evt
            |> System.Text.Encoding.ASCII.GetString
            |> JsonConvert.DeserializeObject<Events>

        let getMetadataArray =
            Array.zeroCreate 0

        let resolvedEventToEvent (revt: ResolvedEvent) = 
            revt.OriginalEvent.Data
            |> getEvent

        let mapEventSliceToEvents (slice: StreamEventsSlice) = 
            slice.Events
            |> Array.map resolvedEventToEvent
            
        let mapAllEventSliceToEvents (slice: AllEventsSlice) = 
            slice.Events
            |> Array.map resolvedEventToEvent

        member x.Settings = settings
        member x.Store (streamId: string) (evts: Events list) = async {
            try
                use conn = EventStoreConnection.Create x.Settings.ConnectionString
                
                Async.AwaitTask(conn.ConnectAsync())
                |> Async.RunSynchronously
                
                let res = 
                    evts 
                    |> List.map (fun evt -> EventData(Guid.NewGuid(), evt.GetType().ToString(), true, getDataArray evt, getMetadataArray))
                    |> (fun mapped -> Async.AwaitTask(conn.AppendToStreamAsync(streamId, ExpectedVersion.Any, mapped)))
                    |> Async.RunSynchronously 
                    |> ignore
                
                return RResult.rgood res
            with
            | ex -> 
                return RResult.rexn ex
        }

        member x.GetEvents (streamId: string) = async {
            try 
                use conn = EventStoreConnection.Create x.Settings.ConnectionString
                
                Async.AwaitTask(conn.ConnectAsync())
                |> Async.RunSynchronously

                let res = 
                    Async.AwaitTask(conn.ReadStreamEventsForwardAsync(streamId, int64(0), 1000, false))
                    |> Async.RunSynchronously 
                    |> mapEventSliceToEvents
                
                return RResult.rgood res
            with
            | ex -> 
                return RResult.rexn ex
        }

        member x.GetAll = async {
            try 
                use conn = EventStoreConnection.Create x.Settings.ConnectionString
                
                Async.AwaitTask(conn.ConnectAsync())
                |> Async.RunSynchronously

                let res = 
                    Async.AwaitTask(conn.ReadAllEventsForwardAsync(Position.Start, 1000, false))
                    |> Async.RunSynchronously 
                    |> mapAllEventSliceToEvents
                
                return RResult.rgood res
            with
            | ex -> 
                return RResult.rexn ex
        }