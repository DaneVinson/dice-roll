module Dice.Api.Tests.RollHandlerTests

open System.IO
open System.Threading.Tasks
open Expecto
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Dice.Api.DiceRoutes

let private createContext () =
    let services = ServiceCollection()
    services.AddGiraffe() |> ignore
    let ctx = DefaultHttpContext()
    ctx.RequestServices <- services.BuildServiceProvider()
    ctx.Response.Body <- new MemoryStream()
    ctx

let private invoke (handler: HttpHandler) =
    let ctx = createContext ()
    handler (fun c -> Task.FromResult(Some c)) ctx
    |> Async.AwaitTask
    |> Async.RunSynchronously

let private responseBody (ctx: HttpContext) =
    ctx.Response.Body.Seek(0L, SeekOrigin.Begin) |> ignore
    use reader = new StreamReader(ctx.Response.Body)
    reader.ReadToEnd()

let private diceTests name (rollFn: int -> HttpHandler) maxValue =
    testList name [
        test "returns 400 for count of 0" {
            let ctx = invoke (rollFn 0) |> Option.get
            Expect.equal ctx.Response.StatusCode 400 "Should return 400"
        }
        test "returns 400 for count of 101" {
            let ctx = invoke (rollFn 101) |> Option.get
            Expect.equal ctx.Response.StatusCode 400 "Should return 400"
        }
        test "returns 400 for a negative count" {
            let ctx = invoke (rollFn -5) |> Option.get
            Expect.equal ctx.Response.StatusCode 400 "Should return 400"
        }
        test "returns correct error message for invalid count" {
            let ctx = invoke (rollFn 0) |> Option.get
            Expect.stringContains (responseBody ctx) "Count must be between 1 and 100." "Should include error message"
        }
        test "returns 200 for count of 1" {
            let ctx = invoke (rollFn 1) |> Option.get
            Expect.equal ctx.Response.StatusCode 200 "Should return 200"
        }
        test "returns 200 for count of 100" {
            let ctx = invoke (rollFn 100) |> Option.get
            Expect.equal ctx.Response.StatusCode 200 "Should return 200"
        }
        test "returns array with correct length" {
            let count = 10
            let ctx = invoke (rollFn count) |> Option.get
            let rolls = System.Text.Json.JsonSerializer.Deserialize<int[]>(responseBody ctx)
            Expect.equal rolls.Length count "Array length should match requested count"
        }
        test "returns values within valid range" {
            let ctx = invoke (rollFn 100) |> Option.get
            let rolls = System.Text.Json.JsonSerializer.Deserialize<int[]>(responseBody ctx)
            for roll in rolls do
                Expect.isGreaterThanOrEqual roll 1 "Value should be at least 1"
                Expect.isLessThanOrEqual roll maxValue $"Value should be at most {maxValue}"
        }
    ]

[<Tests>]
let allTests =
    testList "Dice Roll Handlers" [
        diceTests "D4"   RollD4.roll   4
        diceTests "D6"   RollD6.roll   6
        diceTests "D8"   RollD8.roll   8
        diceTests "D10"  RollD10.roll  10
        diceTests "D12"  RollD12.roll  12
        diceTests "D20"  RollD20.roll  20
        diceTests "D100" RollD100.roll 100
    ]
