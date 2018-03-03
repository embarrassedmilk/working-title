namespace WorkingTitle.Domain.Accounts.Commands

open WorkingTitle.Domain.Primitives
open WorkingTitle.Domain.EventSource
open WorkingTitle.Domain.Accounts.Events
open System
open WorkingTitle.Utils.RResult

type ChangeAccountUsername(id: string, username: string) =
    member this.Id = id
    member this.Username = username
    member this.ToEvent() = 
        RResult.rgood (AccountUsernameChanged(id, DateTimeOffset.UtcNow, this.Username))
        |>> UsernameChanged