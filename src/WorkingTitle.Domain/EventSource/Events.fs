namespace WorkingTitle.Domain.EventSource

open System
open WorkingTitle.Domain.Accounts.Events

type Events =
    | Created of AccountCreated
    | EmailChanged of AccountEmailChanged
    | UsernameChanged of AccountUsernameChanged
    member x.EntityId : Guid = 
        match x with 
        | Created created -> created.EntityId
        | EmailChanged emailChanged -> emailChanged.EntityId
        | UsernameChanged usernameChanged -> usernameChanged.EntityId