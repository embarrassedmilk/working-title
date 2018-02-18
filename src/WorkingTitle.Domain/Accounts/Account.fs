namespace WorkingTitle.Domain.Accounts

open WorkingTitle.Domain.Accounts.Events
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Accounts.Snapshots
open WorkingTitle.Domain.Accounts.Commands

type Account() = 
    static member ApplyCreate (evt: AccountCreated) =
        AccountState(evt.EntityId, evt.Email, evt.Username)

    static member Apply (state: AccountState) (evt: ICmsEvent) = 
        match evt with
        | :? AccountEmailChanged as emailChanged ->
            state.WithEmail(emailChanged.Email)
        | :? AccountUsernameChanged as usernameChanged ->
            state.WithUsername(usernameChanged.Username)
        | _ -> 
            state

    static member Create (cmd:CreateAccount) = 
        let evt = cmd.ToEvent()
        let state = Account.ApplyCreate(evt)
        (evt, state)

    static member ChangeEmail (currentState:AccountState) (cmd:ChangeAccountEmail) =
        let evt = cmd.ToEvent()
        let state = Account.Apply currentState evt
        (evt,state)
    
    static member ChangeUsername (currentState:AccountState) (cmd:ChangeAccountUsername) =
        let evt = cmd.ToEvent()
        let state = Account.Apply currentState evt
        (evt,state)
