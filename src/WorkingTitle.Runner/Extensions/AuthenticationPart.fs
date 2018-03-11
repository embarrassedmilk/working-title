namespace WorkingTitle.Runner.Extensions

open Snake.Core
open Microsoft.AspNetCore.Builder;
open Microsoft.AspNetCore.Hosting;
open Microsoft.Extensions.DependencyInjection;
open Microsoft.AspNetCore.Authentication.Cookies;
open Microsoft.AspNetCore.Authentication.Google;
open Microsoft.AspNetCore.Authentication.OAuth;
open System.Collections.Generic
open System.Threading.Tasks
open System
open System.Web
open Microsoft.AspNetCore.Authentication
open System.Text.Encodings.Web
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Authentication
open System.Net
open Microsoft.AspNetCore.Authentication

type AuthenticationPart() = 
    interface IApplicationPlugin with
        member x.Configure(app:IApplicationBuilder, env:IHostingEnvironment) = 
            app.UseAuthentication()
            |> ignore

        member x.ConfigureServices(services:IServiceCollection) =
            services
                .AddAuthentication(fun opts -> 
                    opts.DefaultScheme  <- CookieAuthenticationDefaults.AuthenticationScheme
                )
                .AddCookie(fun opts -> 
                    opts.Cookie.SameSite            <- Microsoft.AspNetCore.Http.SameSiteMode.None
                    opts.Cookie.HttpOnly            <- true
                    opts.LoginPath                  <- new PathString("/api/account/login")
                    opts.LogoutPath                 <- new PathString("/api/account/logout")
                    opts.Events.OnRedirectToLogin   <- new Func<RedirectContext<CookieAuthenticationOptions>, Task>(fun ctxt -> 
                        ctxt.Response.StatusCode <- 401
                        Task.CompletedTask
                    )
                )
                .AddGoogle(fun opts -> 
                    opts.ClientId       <- ""
                    opts.ClientSecret   <- ""
                )
            |> ignore

        member x.BeforeBuild(app:IApplicationBuilder, env:IHostingEnvironment, services:IEnumerable<ServiceDescriptor>) = 
            ()