namespace Owl.Edge.WebDriver

open System
open System.Runtime
open System.Net
open System.Net.Http
open System.IO

module Trunk =
  let client = new HttpClient()

  type Platform = None = -1 | Windows = 0 | Mac = 1 | Linux = 2
  type Architecture = None = -1 | x86 = 0 | x64 = 1 | ARM = 2

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
    | _ -> raise (exn $"{Environment.OSVersion.Platform} is not supported.")

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
    }

  let inline download version =
    let file_name = Path.Combine(Directory.GetCurrentDirectory(), zip_name)
    downloadAs (file_name, version)