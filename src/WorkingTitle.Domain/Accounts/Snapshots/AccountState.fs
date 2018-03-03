namespace WorkingTitle.Domain.Accounts.Snapshots
open System
open WorkingTitle.Domain.Primitives

type ExistingAccountState(id: string, email: string, username: string) = 
    member this.Email = email
    member this.Username = username
    member this.Id = id

    member this.WithUsername (username: string) =
        ExistingAccountState(this.Id, this.Email, username)
    
    member this.WithEmail (email: string) =
        ExistingAccountState(this.Id, email, this.Username)

type AccountState = 
    | ExistingAccount of ExistingAccountState 
    | NonExistingAccount