namespace Owl.Edge.WebDriver

open System
open System.Runtime
open System.Net
open System.Net.Http
open System.IO
open System.Diagnostics
open System.Text.RegularExpressions
open System.Threading.Tasks

module Trunk =
  [<Literal>]
  let pattern = "<string>(.*?)</string>"

  let client = new HttpClient()

  type Platform = None = -1 | Windows = 0 | Mac = 1 | Linux = 2
  type Architecture = None = -1 | x86 = 0 | x64 = 1 | ARM = 2

  [<System.Runtime.Versioning.SupportedOSPlatform("Windows")>]
  [<System.Runtime.Versioning.SupportedOSPlatform("MacCatalyst")>]
  [<System.Runtime.Versioning.SupportedOSPlatform("Linux")>]
  let platform =
    if OperatingSystem.IsWindows() then Platform.Windows
    else if OperatingSystem.IsMacOS() then Platform.Mac
    else if OperatingSystem.IsLinux() then Platform.Linux
    else Platform.None

  let architecture =
    if Intrinsics.X86.X86Base.X64.IsSupported then Architecture.x64
    else if Intrinsics.X86.X86Base.IsSupported then Architecture.x86
    else if Intrinsics.Arm.ArmBase.Arm64.IsSupported then Architecture.ARM
    else Architecture.None
    
  [<System.Runtime.Versioning.SupportedOSPlatform("Windows")>]
  [<System.Runtime.Versioning.SupportedOSPlatform("MacCatalyst")>]
  let stable : FileSystemInfo =
    match platform with
    | Platform.Windows -> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe") |> FileInfo :> FileSystemInfo
    | Platform.Mac -> "/Applications/Microsoft Edge.app" |> DirectoryInfo :> FileSystemInfo
    | _ -> raise (exn $"{platform} is not supported.")
  [<System.Runtime.Versioning.SupportedOSPlatform("Windows")>]
  [<System.Runtime.Versioning.SupportedOSPlatform("MacCatalyst")>]
  let beta : FileSystemInfo = 
    match platform with
    | Platform.Windows -> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge Beta\Application\msedge.exe") |> FileInfo :> FileSystemInfo
    | Platform.Mac -> "/Applications/Microsoft Edge Beta.app" |> DirectoryInfo :> FileSystemInfo
    | _ -> raise (exn $"{platform} is not supported.")
  [<System.Runtime.Versioning.SupportedOSPlatform("Windows")>]
  [<System.Runtime.Versioning.SupportedOSPlatform("MacCatalyst")>]
  let dev : FileSystemInfo =
    match platform with
    | Platform.Windows -> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge Dev\Application\msedge.exe") |> FileInfo :> FileSystemInfo
    | Platform.Mac -> "/Applications/Microsoft Edge Dev.app" |> DirectoryInfo :> FileSystemInfo
    | _ -> raise (exn $"{platform} is not supported.")
  [<System.Runtime.Versioning.SupportedOSPlatform("Windows")>]
  [<System.Runtime.Versioning.SupportedOSPlatform("MacCatalyst")>]
  let canary =
    match platform with
    | Platform.Windows -> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge SxS\Application\msedge.exe") |> FileInfo :> FileSystemInfo
    | Platform.Mac -> "/Applications/Microsoft Edge Canary.app" |> DirectoryInfo :> FileSystemInfo
    | _ -> raise (exn $"{platform} is not supported.")
  
  [<System.Runtime.Versioning.SupportedOSPlatform("MacCatalyst")>]
  let inline find_version (info: FileSystemInfo) =
    let readLines = Path.Combine >> File.ReadLines >> (fun iter -> iter.GetEnumerator())
    let e = [| info.FullName; "Contents/info.plist" |] |> readLines
    let mutable is_break = false
    let mutable found = false
    let mutable version = ""
    while not is_break && e.MoveNext() do
      if e.Current.Contains "CFBundleShortVersionString" then found <- true
      else if found then
        version <- Regex.Match(e.Current, pattern).Groups[1] |> string
        is_break <- true
      else
        ()
    if System.String.IsNullOrEmpty version
    then raise (exn $"Version information is not found.")
    else version
    
  [<System.Runtime.Versioning.SupportedOSPlatform("Windows")>]
  [<System.Runtime.Versioning.SupportedOSPlatform("MacCatalyst")>]
  let inline get_version (info: FileSystemInfo) =
    match platform with
    | Platform.Windows -> (FileVersionInfo.GetVersionInfo info.FullName).ProductVersion
    | Platform.Mac -> find_version info
    | _ -> raise (exn $"{platform} is not supported.")

  let inline get_x64_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_win64.zip"
  let inline get_x86_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_win32.zip"
  let inline get_linux_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_linux64.zip"
  let inline get_mac_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_mac64.zip"
  let inline get_arm_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_arm64.zip"

  let zip_name = 
    match (platform, architecture) with
    | (Platform.Windows, Architecture.x64) -> "edgedriver_win64.zip"
    | (Platform.Windows, Architecture.x86) -> "edgedriver_win32.zip"
    | (Platform.Windows, Architecture.ARM) -> "edgedriver_arm64.zip"
    | (Platform.Mac, _) -> "edgedriver_mac64.zip"
    | (Platform.Linux, _) -> "edgedriver_linux64.zip"
    | _ -> raise (exn $"{platform} ({architecture}) is not supported.")

  let inline downloadAs (saveAs, version) =
    let url =
      match (platform, architecture) with
      | (Platform.Windows, Architecture.x64) -> get_x64_driver_url version
      | (Platform.Windows, Architecture.x86) -> get_x86_driver_url version
      | (Platform.Windows, Architecture.ARM) -> get_arm_driver_url version
      | (Platform.Mac, _) -> get_mac_driver_url version
      | (Platform.Linux, _) -> get_linux_driver_url version
      | _ -> raise (exn $"{Environment.OSVersion.Platform} is not supported.")
    use request = new HttpRequestMessage(HttpMethod.Get, Uri url)
    task {
      use! response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
      if response.StatusCode = HttpStatusCode.OK then
        use content = response.Content
        use! stream = content.ReadAsStreamAsync()
        use filestream = new FileStream(saveAs, FileMode.Create, FileAccess.Write, FileShare.None)
        stream.CopyTo filestream
        return FileInfo saveAs
      else 
        return raise (exn $"{Environment.OSVersion.Platform} is not supported.")
    }

  let inline download version =
    let file_name = Path.Combine(Directory.GetCurrentDirectory(), zip_name)
    downloadAs (file_name, version)

  let inline unzip (zip: Task<FileInfo>) =
    task {
      let! file = zip
      System.IO.Compression.ZipFile.ExtractToDirectory(file.FullName, file.DirectoryName)
      return DirectoryInfo file.DirectoryName
    }

  let inline remove (zip: Task<FileInfo>) =
    task {
      let! file = zip
      file.Delete()
    }

  let inline sync<'T> (task: Task<'T>) = task |> (Async.AwaitTask >> Async.RunSynchronously)
