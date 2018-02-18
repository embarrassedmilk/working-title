namespace WorkingTitle.Domain.Accounts.Snapshots

open WorkingTitle.Domain.Primitives

type AccountState(email: Email, username: Username) = 
    member this.Email = email
    member this.Username = username

    member this.WithUsername(username: Username) =
        AccountState(this.Email, username)
    
    member this.WithEmail(email: Email) =
        AccountState(email, this.Username)