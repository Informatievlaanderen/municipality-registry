#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.3 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.FileSystemOperators
open ``Build-generic``

let product = "Basisregisters Vlaanderen"
let copyright = "Copyright (c) Vlaamse overheid"
let company = "Vlaamse overheid"

let dockerRepository = "municipality-registry"
let assemblyVersionNumber = (sprintf "2.%s")
let nugetVersionNumber = (sprintf "%s")

let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let test = testSolution
let publishSource = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "msil"; "linux-x64" ]

// Solution -----------------------------------------------------------------------

Target.create "Restore_Solution" (fun _ -> restore "MunicipalityRegistry")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  buildSource "MunicipalityRegistry.Projector"
  buildSource "MunicipalityRegistry.Producer"
  buildSource "MunicipalityRegistry.Api.Legacy"
  buildSource "MunicipalityRegistry.Api.Oslo"
  buildSource "MunicipalityRegistry.Api.Extract"
  buildSource "MunicipalityRegistry.Api.CrabImport"
  buildSource "MunicipalityRegistry.Projections.Legacy"
  buildSource "MunicipalityRegistry.Projections.Extract"
  buildSource "MunicipalityRegistry.Projections.LastChangedList"
  buildSource "MunicipalityRegistry.Projections.Wfs"
  buildSource "MunicipalityRegistry.Projections.Wms"
  buildTest "MunicipalityRegistry.Projections.Legacy.Tests"
  buildTest "MunicipalityRegistry.Tests"
)

Target.create "Test_Solution" (fun _ ->
    [
        "test" @@ "MunicipalityRegistry.Tests"
        "test" @@ "MunicipalityRegistry.Projections.Legacy.Tests"
    ] |> List.iter testWithDotNet
)


Target.create "Publish_Solution" (fun _ ->
  [
    "MunicipalityRegistry.Projector"
    "MunicipalityRegistry.Producer"
    "MunicipalityRegistry.Api.Legacy"
    "MunicipalityRegistry.Api.Oslo"
    "MunicipalityRegistry.Api.Extract"
    "MunicipalityRegistry.Api.CrabImport"
    "MunicipalityRegistry.Projections.Legacy"
    "MunicipalityRegistry.Projections.Extract"
    "MunicipalityRegistry.Projections.LastChangedList"
    "MunicipalityRegistry.Projections.Wfs"
    "MunicipalityRegistry.Projections.Wms"
  ] |> List.iter publishSource)

Target.create "Pack_Solution" (fun _ ->
  [
    "MunicipalityRegistry.Projector"
    "MunicipalityRegistry.Producer"
    "MunicipalityRegistry.Api.Legacy"
    "MunicipalityRegistry.Api.Oslo"
    "MunicipalityRegistry.Api.Extract"
    "MunicipalityRegistry.Api.CrabImport"
  ] |> List.iter pack)

Target.create "Containerize_ApiProjector" (fun _ -> containerize "MunicipalityRegistry.Projector" "projector")
Target.create "PushContainer_ApiProjector" (fun _ -> push "projector")

Target.create "Containerize_ApiProducer" (fun _ -> containerize "MunicipalityRegistry.Producer" "producer")
Target.create "PushContainer_ApiProducer" (fun _ -> push "producer")

Target.create "Containerize_ApiLegacy" (fun _ -> containerize "MunicipalityRegistry.Api.Legacy" "api-legacy")
Target.create "PushContainer_ApiLegacy" (fun _ -> push "api-legacy")

Target.create "Containerize_ApiOslo" (fun _ -> containerize "MunicipalityRegistry.Api.Oslo" "api-oslo")
Target.create "PushContainer_ApiOslo" (fun _ -> push "api-oslo")

Target.create "Containerize_ApiExtract" (fun _ -> containerize "MunicipalityRegistry.Api.Extract" "api-extract")
Target.create "PushContainer_ApiExtract" (fun _ -> push "api-extract")

Target.create "Containerize_ApiCrabImport" (fun _ -> containerize "MunicipalityRegistry.Api.CrabImport" "api-crab-import")
Target.create "PushContainer_ApiCrabImport" (fun _ -> push "api-crab-import")

// --------------------------------------------------------------------------------

Target.create "Build" ignore
Target.create "Test" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore
Target.create "Push" ignore

"NpmInstall"
  ==> "DotNetCli"
  ==> "Clean"
  ==> "Restore_Solution"
  ==> "Build_Solution"
  ==> "Build"

"Build"
  ==> "Test_Solution"
  ==> "Test"

"Test"
  ==> "Publish_Solution"
  ==> "Publish"

"Publish"
  ==> "Pack_Solution"
  ==> "Pack"

"Pack"
  ==> "Containerize_ApiProjector"
  ==> "Containerize_ApiProducer"
  ==> "Containerize_ApiLegacy"
  ==> "Containerize_ApiOslo"
  ==> "Containerize_ApiExtract"
  ==> "Containerize_ApiCrabImport"
  ==> "Containerize"
// Possibly add more projects to containerize here

"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_ApiProjector"
  ==> "PushContainer_ApiProducer"
  ==> "PushContainer_ApiLegacy"
  ==> "PushContainer_ApiOslo"
  ==> "PushContainer_ApiExtract"
  ==> "PushContainer_ApiCrabImport"
  ==> "Push"
// Possibly add more projects to push here

// By default we build & test
Target.runOrDefault "Test"
