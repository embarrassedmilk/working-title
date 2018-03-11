namespace WorkingTitle.Runner

open Snake.Core

type AuthenticationSettings() =
    member val ClientId = "" with get,set
    member val ClientSecret = "" with get,set

type Settings() = 
    inherit BaseSettings()
    
    member val EventSourceConnectionString = "" with get,set
    member val AuthenticationSettings = new AuthenticationSettings() with get,set