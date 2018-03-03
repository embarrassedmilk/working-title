namespace WorkingTitle.Runner

open Snake.Core

type Settings() = 
    inherit BaseSettings()
    
    member val EventSourceConnectionString = "" with get,set