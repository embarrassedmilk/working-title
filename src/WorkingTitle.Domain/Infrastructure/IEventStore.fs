namespace WorkingTitle.Domain.Infrastructure

open System
open WorkingTitle.Domain.EventSource

type IEventWriter = 
    abstract member Store : event:ICmsEvent -> Unit

type IEventReader = 
    abstract member Get : entityId:Guid -> ICmsEvent list