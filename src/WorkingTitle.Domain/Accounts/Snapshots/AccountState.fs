namespace WorkingTitle.Domain.Accounts.Snapshots
open System
open WorkingTitle.Domain.Primitives

type ExistingAccountState(id: Guid, email: Email, username: Username) = 
    member this.Email = email
    member this.Username = username
    member this.Id = id

    member this.WithUsername (username: Username) =
        ExistingAccountState(this.Id, this.Email, username)
    
    member this.WithEmail (email: Email) =
        ExistingAccountState(this.Id, email, this.Username)

type AccountState = 
    | ExistingAccount of ExistingAccountState 
    | NonExistingAccount