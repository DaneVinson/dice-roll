module Dice.Api.Program

open Giraffe
open Saturn
open Dice.Api.DiceRoutes

let webApp =
    router {
        get "/" (text "Dice.Api Saturn service is running")
        getf "/roll/%i/d/4" RollD4.roll
        getf "/roll/%i/d/6" RollD6.roll
        getf "/roll/%i/d/8" RollD8.roll
        getf "/roll/%i/d/10" RollD10.roll
        getf "/roll/%i/d/12" RollD12.roll
        getf "/roll/%i/d/20" RollD20.roll
        getf "/roll/%i/d/100" RollD100.roll
    }

[<EntryPoint>]
let main _ =
    application {
        use_router webApp
    }
    |> run

    0
