namespace WorkingTitle.RabbitMQ

open System
open RabbitMQ.Client
open System
open WorkingTitle.Utils.RResult
open Newtonsoft.Json
open WorkingTitle.Domain.EventSource
open RabbitMQ.Client.Events

type MessageQueueSettings (connectionString: string, queueName: string) =
    member x.ConnectionString = connectionString
    member x.QueueName = queueName

type MessageQueueWriter(queueSettings: MessageQueueSettings) = 
    let getByteArray (evt: Events) =
        JsonConvert.SerializeObject evt 
        |> System.Text.Encoding.ASCII.GetBytes

    member x.Settings = queueSettings
    
    member x.Publish (evt: Events) =
        try
            let factory = new ConnectionFactory()
            factory.Uri <- new Uri(x.Settings.ConnectionString)
            use conn = factory.CreateConnection()
            let channel = conn.CreateModel()
            let messageBodyBytes = getByteArray evt
            channel.BasicPublish(String.Empty, x.Settings.QueueName, null, messageBodyBytes)

            RResult.rgood ()
        with
        | ex -> 
            RResult.rexn ex

type MessageQueueReader(queueSettings: MessageQueueSettings) = 
    let getEvent (evt: byte[]) =
        evt
        |> System.Text.Encoding.ASCII.GetString
        |> JsonConvert.DeserializeObject<Events>

    member x.Settings = queueSettings

    member x.Subscribe (f: Events -> unit) =
        try
            let factory = new ConnectionFactory()
            factory.Uri <- new Uri(x.Settings.ConnectionString)
            use conn = factory.CreateConnection()
            let channel = conn.CreateModel()
            let consumer = new EventingBasicConsumer(channel)
            consumer.Received.Add(fun (ea) -> 
                let body = ea.Body
                f (getEvent body)
                channel.BasicAck(ea.DeliveryTag, false)
            )

            RResult.rgood ()
        with
        | ex -> 
            RResult.rexn ex