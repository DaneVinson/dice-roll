module Dice.Api.Program

open System.IO
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Dice.Api.DiceRoutes

let webApp =
    choose [
        GET
        >=> choose [
            routef "/roll/%i/d/4" RollD4.roll
            routef "/roll/%i/d/6" RollD6.roll
            routef "/roll/%i/d/8" RollD8.roll
            routef "/roll/%i/d/10" RollD10.roll
            routef "/roll/%i/d/12" RollD12.roll
            routef "/roll/%i/d/20" RollD20.roll
            routef "/roll/%i/d/100" RollD100.roll
            fun next ctx ->
                let path = ctx.Request.Path.Value
                if
                    not (isNull path)
                    && not (path.StartsWith("/roll", System.StringComparison.OrdinalIgnoreCase))
                    && not (Path.HasExtension(path))
                then
                    htmlFile "wwwroot/index.html" next ctx
                else
                    RequestErrors.NOT_FOUND "Not Found" next ctx
        ]
    ]

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.Services.AddGiraffe() |> ignore

    let app = builder.Build()
    app.UseDefaultFiles() |> ignore
    app.UseStaticFiles() |> ignore
    app.UseGiraffe(webApp)
    app.Run()

    0
