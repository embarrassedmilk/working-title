namespace Snake.FRunner.Controllers

open Microsoft.AspNetCore.Mvc

[<Route("api/[controller]")>]
type TestController () =
    inherit Controller()

    [<HttpGet>]
    member __.Get() =
        ["1", "2"]