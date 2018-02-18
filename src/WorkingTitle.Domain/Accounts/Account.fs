namespace WorkingTitle.Domain.Accounts

open WorkingTitle.Domain.Accounts.Events
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Accounts.Snapshots

type Account() = 
    static member Create(evt: AccountCreated) =
        AccountState(evt.Email, evt.Username)

    static member Apply(state: AccountState, evt: ICmsEvent) = 
        match evt with
        | :? AccountEmailChanged as emailChanged ->
            state.WithEmail(emailChanged.Email)
        | :? AccountUsernameChanged as usernameChanged ->
            state.WithUsername(usernameChanged.Username)
        | _ -> 
            state