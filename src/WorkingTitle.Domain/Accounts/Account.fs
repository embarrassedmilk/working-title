namespace WorkingTitle.Domain.Accounts

open WorkingTitle.Domain.Accounts.Events
open WorkingTitle.Domain.Accounts.Snapshots
open WorkingTitle.Domain.Accounts.Commands
open WorkingTitle.Domain.EventSource

type Account() = 
    static member Apply (s: AccountState) (evt: ICmsEvent) = 
        match s, evt with
        | NonExistingAccount _, (:? AccountCreated as accountCreated) ->
            let newState = ExistingAccountState(accountCreated.EntityId, accountCreated.Email, accountCreated.Username)
            ExistingAccount newState
        | ExistingAccount state, (:? AccountEmailChanged as emailChanged) ->
            let newState = state.WithEmail(emailChanged.Email)
            ExistingAccount newState
        | ExistingAccount state, (:? AccountUsernameChanged as usernameChanged) ->
            let newState = state.WithUsername(usernameChanged.Username)
            ExistingAccount newState
        | _,_ ->
            s

    static member Create (cmd:CreateAccount) = 
        let evt = cmd.ToEvent()
        let state = Account.Apply NonExistingAccount evt
        (evt, state)

    static member ChangeEmail (currentState:AccountState) (cmd:ChangeAccountEmail) =
        let evt = cmd.ToEvent()
        let state = Account.Apply currentState evt
        (evt,state)
    
    static member ChangeUsername (currentState:AccountState) (cmd:ChangeAccountUsername) =
        let evt = cmd.ToEvent()
        let state = Account.Apply currentState evt
        (evt,state)

    static member GetAccountStateFromEvents (evts:ICmsEvent list) =
        evts |> List.fold Account.Apply NonExistingAccount
