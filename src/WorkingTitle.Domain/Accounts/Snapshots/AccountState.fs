namespace WorkingTitle.Domain.Accounts.Snapshots
open System
open WorkingTitle.Domain.Primitives

type AccountState(id: Guid, email: Email, username: Username) = 
    member this.Email = email
    member this.Username = username
    member this.Id = id

    member this.WithUsername (username: Username) =
        AccountState(this.Id, this.Email, username)
    
    member this.WithEmail (email: Email) =
        AccountState(this.Id, email, this.Username)