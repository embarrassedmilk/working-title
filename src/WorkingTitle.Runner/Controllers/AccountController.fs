namespace Snake.FRunner.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.SignalR
open WorkingTitle.Runner.Hubs
open Microsoft.AspNetCore.Authentication;
open Microsoft.AspNetCore.Authentication.Cookies;
open Microsoft.AspNetCore.Authorization;

[<Route("api/[controller]")>]
type AccountController (hc:IHubContext<EventsHub>) =
    inherit Controller()

    [<HttpGet("getuser")>]
    member __.GetUser() =
        __.Ok(__.User.Identity.Name)

    [<HttpGet("login")>]
    [<AllowAnonymous>]
    member __.Login() =
        let ap = new AuthenticationProperties()
        ap.RedirectUri <- "http://localhost:8080"
        __.Challenge(ap, "Google")

    [<HttpGet("logout")>]
    member __.Logout() = 
        let ap = new AuthenticationProperties()
        ap.RedirectUri <- "http://localhost:8080"
        __.SignOut(ap, CookieAuthenticationDefaults.AuthenticationScheme)