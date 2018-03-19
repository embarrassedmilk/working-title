namespace WorkingTitle.Redis

open System
open WorkingTitle.Domain.Posts.Snapshots
open StackExchange.Redis
open Newtonsoft.Json
open WorkingTitle.RabbitMQ
open WorkingTitle.Persistence.EventStore.Store
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Posts
open WorkingTitle.Utils.RResult
open System.Collections.Generic

type CacheSettings (connectionString: string) =
    member x.ConnectionString = connectionString

type Cache(cacheSettings: CacheSettings) = 
    let getKeys (server: IServer) =
        server.Keys()
        |> Seq.map (fun k -> k.ToString())

    let getPost (serializedPost: string) =
        serializedPost |> JsonConvert.DeserializeObject<PostState>

    let serializePost (post: PostState) =
        JsonConvert.SerializeObject post

    let getPostByKey (muxer: ConnectionMultiplexer) (key: string)  = async {
        let database = muxer.GetDatabase()
        let! value = database.StringGetAsync(RedisKey.op_Implicit(key)) |> Async.AwaitTask
        return getPost (value.ToString())
    }

    let savePost (muxer: ConnectionMultiplexer) (key: string, post: PostState) = async {
        let database = muxer.GetDatabase()
        database.StringSetAsync(
            [| new KeyValuePair<RedisKey, RedisValue>(RedisKey.op_Implicit(key), RedisValue.op_Implicit(serializePost(post))) |]
        ) |> Async.AwaitTask |> ignore
    }

    let getEvents (store:Store) =
        store.GetAll
        |> Async.RunSynchronously
        |>> Array.toList

    let groupById (evts: Events list) =
        evts |> List.groupBy (fun evt -> evt.EntityId)

    let replayPostStream (entityId: string, evts: Events list) =
        (entityId, evts |> Post.ReplayPost)

    member x.Settings = cacheSettings

    member x.GetAllPosts = async {
        try
            let! conn = ConnectionMultiplexer.ConnectAsync(x.Settings.ConnectionString) |> Async.AwaitTask
            let res = 
                conn.GetEndPoints() 
                    |> Array.find (fun ep -> (not (conn.GetServer(ep).IsSlave)))
                    |> conn.GetServer
                    |> getKeys
                    |> Seq.map (getPostByKey conn >> Async.RunSynchronously)
            
            return RResult.rgood res
        with
        | ex -> 
            return RResult.rexn ex
    }

    member x.GetPostById (key: string) = async {
        try
            let! conn = ConnectionMultiplexer.ConnectAsync(x.Settings.ConnectionString) |> Async.AwaitTask
            let! res = getPostByKey conn key

            return RResult.rgood res
        with
        | ex -> 
            return RResult.rexn ex
    }

    member x.Subscribe(queue: MessageQueueReader) = async {
        try
            let! conn = ConnectionMultiplexer.ConnectAsync(x.Settings.ConnectionString) |> Async.AwaitTask
            return queue.Subscribe(fun evt -> 
                x.GetPostById evt.EntityId |> Async.RunSynchronously
                |>> fun state -> Post.Apply state evt
                |>> fun newState -> (savePost conn (evt.EntityId, newState)) |> Async.RunSynchronously
                |> ignore
            )
        with
        | ex -> 
            return RResult.rexn ex
    }

    member x.Rehydrate(store: Store) = async { //TODO: use traverse here
        try
            let! conn = ConnectionMultiplexer.ConnectAsync(x.Settings.ConnectionString) |> Async.AwaitTask
            return getEvents store
                    |>> groupById
                    |>> List.map replayPostStream
                    |>> List.map (savePost conn)
                    |>> List.map Async.RunSynchronously
        with
        | ex -> 
            return RResult.rexn ex
    }