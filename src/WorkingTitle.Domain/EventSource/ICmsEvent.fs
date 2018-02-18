namespace WorkingTitle.Domain.EventSource

open System

type ICmsEvent = 
    abstract member EntityId: Guid with get
    abstract member Timestamp: DateTimeOffset with get