// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.
open System.IO
open System
open System.Diagnostics
open System.Net.Http
open System.IO.Compression


let downloadFile (url: string) (outputPath: string) =
    async {
        use client = new HttpClient()
        let! response = client.GetAsync(url) |> Async.AwaitTask
        response.EnsureSuccessStatusCode() |> ignore
        let! content = response.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
        File.WriteAllBytes(outputPath, content)
    } |> Async.Ignore
let getSolutionDirectory() =
    let mutable result = Directory.GetCurrentDirectory()
    let mutable x = true
    while x do 
        let dir = DirectoryInfo(result).Parent
        result <- dir.FullName
        if dir.Name = "TCPLib" then
            x <- false
    result
let downloadProtoc() =
    let temp = Path.GetTempFileName()
    printfn "Download protoc..."
    if OperatingSystem.IsWindows() then
        if Environment.Is64BitOperatingSystem then
            downloadFile "https://github.com/protocolbuffers/protobuf/releases/download/v28.0/protoc-28.0-win64.zip" temp |> Async.RunSynchronously
        else 
            downloadFile "https://github.com/protocolbuffers/protobuf/releases/download/v28.0/protoc-28.0-win32.zip" temp |> Async.RunSynchronously
        let archive = ZipFile.Open(temp, ZipArchiveMode.Read)
        let protoc = 
            archive.Entries
            |> Seq.find (fun e -> e.Name = "protoc.exe")

        let path = (Path.Combine(getSolutionDirectory(), "protoc.exe"))
        if File.Exists path = false then 
            protoc.ExtractToFile path
        archive.Dispose()
    elif OperatingSystem.IsLinux() then
        if Environment.Is64BitOperatingSystem then
            downloadFile "https://github.com/protocolbuffers/protobuf/releases/download/v28.0/protoc-28.0-linux-x86_64.zip" temp |> Async.RunSynchronously
        else 
            downloadFile "https://github.com/protocolbuffers/protobuf/releases/download/v28.0/protoc-28.0-linux-x86_32.zip" temp |> Async.RunSynchronously
        let archive = ZipFile.Open(temp, ZipArchiveMode.Read)
        let protoc = 
            archive.Entries
            |> Seq.find (fun e -> e.Name = "protoc")
        printfn "Extract protoc..."
        let path = (Path.Combine(getSolutionDirectory(), "protoc"))
        if File.Exists path = false then 
            protoc.ExtractToFile path
        archive.Dispose()

    File.Delete temp
    printfn "protoc downloaded!"

let startProtoc (file: string) (output: string) =
    let startInfo = 
        ProcessStartInfo(
            FileName = 
                if OperatingSystem.IsWindows() then 
                    (Path.Combine(getSolutionDirectory(), "protoc.exe"))
                else 
                    (Path.Combine(getSolutionDirectory(), "protoc"))
                ,
            Arguments = sprintf "--csharp_out=\"%s\" --csharp_opt=file_extension=.proto.cs,serializable %s" output (file.Replace(@"\", "/")),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        )

    use p = Process.Start(startInfo)
    p.WaitForExit()

let args = System.Environment.GetCommandLineArgs()
let output = (Path.Combine(args[2].Trim('"'), "gen"))
let input = Path.Combine(args[2].Trim('"'))

printfn "===Compiling .proto files==="

printfn "Base directory: %s" (Directory.GetCurrentDirectory())
printfn "Source proto files directory: %s" input
printfn "Generated cs files directory: %s" output

printfn "directory restore..."

if Directory.Exists output = false then
    Directory.CreateDirectory output |> ignore
else
    printfn "Cleaning up previously generated files..."
    for file in Directory.GetFiles(output, "*.cs") do
        File.Delete file

let srcs = Directory.GetFiles(input, "*.proto")

printfn "founded .proto files: %i" srcs.Length

if OperatingSystem.IsLinux() then
    if File.Exists (Path.Combine(getSolutionDirectory(), "protoc")) = false then
        downloadProtoc()
    printfn "Compilation..."
    for file in srcs do
        startProtoc file output
elif OperatingSystem.IsWindows() then
    if File.Exists (Path.Combine(getSolutionDirectory(), "protoc.exe")) = false then
        downloadProtoc()
    printfn "Compilation..."
    for file in srcs do
        startProtoc file output
printfn "Completed!"
0
