#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json

nuget Microsoft.Build 17.3.2
nuget Microsoft.Build.Framework 17.3.2
nuget Microsoft.Build.Tasks.Core 17.3.2
nuget Microsoft.Build.Utilities.Core 17.3.2

nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.6 //"

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

let buildSolution = buildSolution assemblyVersionNumber
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
  buildSolution "MunicipalityRegistry")

Target.create "Test_Solution" (fun _ -> test "MunicipalityRegistry")

Target.create "Publish_Solution" (fun _ ->
  [
    "MunicipalityRegistry.Projector"
    "MunicipalityRegistry.Producer"
    "MunicipalityRegistry.Producer.Snapshot.Oslo"
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
    "MunicipalityRegistry.Api.Legacy"
    "MunicipalityRegistry.Api.Oslo"
    "MunicipalityRegistry.Api.Extract"
    "MunicipalityRegistry.Api.CrabImport"
  ] |> List.iter pack)

Target.create "Containerize_Projector" (fun _ -> containerize "MunicipalityRegistry.Projector" "projector")
Target.create "Containerize_Producer" (fun _ -> containerize "MunicipalityRegistry.Producer" "producer")
Target.create "Containerize_ProducerSnapshotOslo" (fun _ -> containerize "MunicipalityRegistry.Producer.Snapshot.Oslo" "producer-snapshot-oslo")
Target.create "Containerize_ApiLegacy" (fun _ -> containerize "MunicipalityRegistry.Api.Legacy" "api-legacy")
Target.create "Containerize_ApiOslo" (fun _ -> containerize "MunicipalityRegistry.Api.Oslo" "api-oslo")
Target.create "Containerize_ApiExtract" (fun _ -> containerize "MunicipalityRegistry.Api.Extract" "api-extract")
Target.create "Containerize_ApiCrabImport" (fun _ -> containerize "MunicipalityRegistry.Api.CrabImport" "api-crab-import")

Target.create "SetAssemblyVersions" (fun _ -> setVersions "SolutionInfo.cs")

// --------------------------------------------------------------------------------

Target.create "Build" ignore
Target.create "Test" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore

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
  //==> "Containerize_Projector"
  //==> "Containerize_Producer"
  //==> "Containerize_ProducerSnapshotOslo"
  //==> "Containerize_ApiLegacy"
  //==> "Containerize_ApiOslo"
  //==> "Containerize_ApiExtract"
  //==> "Containerize_ApiCrabImport"
  ==> "Containerize"
// Possibly add more projects to containerize here

// By default we build & test
Target.runOrDefault "Test"
