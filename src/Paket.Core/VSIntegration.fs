﻿module Paket.VSIntegration

open System.IO
open Logging
open System
open Chessie.ErrorHandling
open Domain
open DownloadAssemblies

let TurnOnAutoRestore environment =
    let exeDir = Path.Combine(environment.RootDirectory.FullName, ".paket")

    trial {         
        let paketTargetsPath = Path.Combine(exeDir, "paket.targets")
        do! downloadPaketAndBootstrapper environment

        environment.Projects
        |> List.map fst
        |> List.iter (fun project ->
            let relativePath = createRelativePath project.FileName paketTargetsPath
            project.AddImportForPaketTargets(relativePath)
            project.Save()
        )
    } 

let TurnOffAutoRestore environment = 
    let exeDir = Path.Combine(environment.RootDirectory.FullName, ".paket")
    
    trial {
        let paketTargetsPath = Path.Combine(exeDir, "paket.targets")
        do! removeFile paketTargetsPath

        environment.Projects
        |> List.map fst
        |> List.iter (fun project ->
            let relativePath = createRelativePath project.FileName paketTargetsPath
            project.RemoveImportForPaketTargets(relativePath)
            project.Save()
        )
    }