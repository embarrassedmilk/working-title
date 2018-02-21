namespace WorkingTitle.Domain.EventSource

open System
open WorkingTitle.Domain.Accounts.Events

type AccountEvents =
    | Created of AccountCreated
    | EmailChanged of AccountEmailChanged
    | UsernameChanged of AccountUsernameChanged

type Events = 
    | Account of AccountEvents