namespace WorkingTitle.Domain.Accounts

open WorkingTitle.Domain.Accounts.Snapshots
open WorkingTitle.Domain.Accounts.Commands
open WorkingTitle.Domain.EventSource

type Account() = 
    static member Apply (s: AccountState) (evt: Events) = 
        match s, evt with
        | NonExistingAccount _, Created accountCreated ->
            let newState = ExistingAccountState(accountCreated.EntityId, accountCreated.Email, accountCreated.Username)
            ExistingAccount newState
        | ExistingAccount state, EmailChanged emailChanged ->
            let newState = state.WithEmail(emailChanged.Email)
            ExistingAccount newState
        | ExistingAccount state, UsernameChanged usernameChanged ->
            let newState = state.WithUsername(usernameChanged.Username)
            ExistingAccount newState
        | _,_ ->
            s

    static member Create (cmd:CreateAccount) = 
        cmd.ToEvent()
        |>> (fun evt -> 
            let state = Account.Apply NonExistingAccount evt
            ([evt], state))

    static member ChangeEmail (cmd:ChangeAccountEmail) (currentState:AccountState) =
        cmd.ToEvent()
        |>> (fun evt -> 
            let state = Account.Apply currentState evt
            ([evt], state))
    
    static member ChangeUsername (cmd:ChangeAccountUsername) (currentState:AccountState) =
        cmd.ToEvent()
        |>> (fun evt -> 
            let state = Account.Apply currentState evt
            ([evt], state))

    static member ReplayAccount (evts:Events list) =
        evts |> List.fold Account.Apply NonExistingAccount
