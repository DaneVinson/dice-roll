module Dice.Api.Tests.Program

open Expecto

[<EntryPoint>]
let main args =
    runTestsWithCLIArgs [] args Dice.Api.Tests.RollHandlerTests.allTests
