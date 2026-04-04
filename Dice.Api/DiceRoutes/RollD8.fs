module Dice.Api.DiceRoutes.RollD8

open System
open Giraffe

let roll (count: int) : HttpHandler =
    if count < 1 || count > 100 then
        setStatusCode 400 >=> text "Count must be between 1 and 100."
    else
        Array.init count (fun _ -> Random.Shared.Next(1, 9))
        |> json