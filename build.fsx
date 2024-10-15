// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

open System.Diagnostics
open System.IO
open System.Xml.Linq
open System.Linq
open System.Text.RegularExpressions

let splitCommandWithQuotes (input: string) =
    let pattern = @"(?<quote>""(?<quoted>[^""]*)""|(?<unquoted>\S+))"
    let matches = Regex.Matches(input, pattern)

    let command = 
        if matches.Count > 0 then
            matches.[0].Value
        else
            ""

    let args =
        [ for m in matches do
            if m.Groups.["quoted"].Success then
                yield m.Groups.["quoted"].Value
            elif m.Groups.["unquoted"].Success then
                yield m.Groups.["unquoted"].Value ]

    (command, args)
let runCommand (command: string) (workdir: string) (disableRaise: bool) =
    let cmnd, args = splitCommandWithQuotes command
    let startInfo = ProcessStartInfo(cmnd, args)
    startInfo.UseShellExecute <- false
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.WorkingDirectory <- workdir

    use p = Process.Start(startInfo)
    let output = p.StandardOutput.ReadToEnd()
    let error = p.StandardError.ReadToEnd()
    p.WaitForExit()
    if p.ExitCode <> 0 && not disableRaise then 
        raise (System.Exception(sprintf "Exit Code: %i;\n StandardOutput: \"%s\";\n StandardError: \"%s\"" p.ExitCode output error))
    sprintf "StandardOutput:%s\nStandardError:%s" output error

let rec getFilesRecursively (directory: string) : string list =
    let files = Directory.GetFiles(directory) |> Array.toList
    

    let subdirectories = Directory.GetDirectories(directory) |> Array.toList


    let subFiles = 
        subdirectories 
        |> List.collect getFilesRecursively


    files @ subFiles
let printHeader (text: string) = 
    let windowWidth = System.Console.WindowWidth
    let textLength = text.Length
    let mutable toWrite = (windowWidth - textLength) / 2

    if toWrite < 0 then
        toWrite <- 0

    for _ in 1 .. toWrite do
        printf "="

    printf "%s" text

    for _ in 1 .. toWrite do
        printf "="


    printfn ""

let formatTimeSpan (ts: System.TimeSpan) =
    let mutable result = ""
    if ts.Days >= 1 then
        result <- result + sprintf " %dd" (int ts.Days)
    if ts.Hours >= 1 then
        result <- result + sprintf " %dh" (int ts.Hours)
    if ts.Minutes >= 1 then
        result <- result + sprintf " %dm" (int ts.Minutes)
    if ts.Seconds >= 1 then
        result <- result + sprintf " %ds" (int ts.Seconds)
    result <- result.Trim(' ')
    result

let startCompilation = System.DateTime.UtcNow
printHeader "State: Compilation .proto files"
let csprojfiles =
    getFilesRecursively(Directory.GetCurrentDirectory())
    |> List.filter (fun x -> x.EndsWith(".csproj"))

let ToDo = csprojfiles.Length

let mutable completed = 0
let Completed() = 
    completed <- completed + 1
    let percentage = (completed |> float) / (ToDo |> float) * 100.0
    System.Console.Title <- (sprintf "%.2f" percentage) + "% done"
for proj in csprojfiles do
    printfn "%s" proj

    let xdoc = XDocument.Load(proj)
    let target = xdoc.Root.Element("PropertyGroup")

    if target <> null then
        let proto = target.Element("Protobuf")
        let fbs = target.Element("Flatbuf")
        if proto <> null && proto.Value = "enable" then
            runCommand (Path.Combine("dotnet fsi \"" + Directory.GetCurrentDirectory(), "build-proto.fsx\" proto")) (FileInfo(proj).DirectoryName) false |> ignore
        if fbs <> null && fbs.Value = "enable" then
            runCommand (Path.Combine("dotnet fsi \"" + Directory.GetCurrentDirectory(), "build-fbs.fsx\" fbs")) (FileInfo(proj).DirectoryName) false |> ignore
            
    Completed()

printfn "Compiled for: %s" (formatTimeSpan(System.DateTime.UtcNow - startCompilation))
System.Console.ForegroundColor <- System.ConsoleColor.Green
printfn "Completed!"
System.Console.ResetColor()
