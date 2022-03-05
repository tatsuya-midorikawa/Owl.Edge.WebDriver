namespace Owl.Edge.WebDriver

module Trunk =
  let get_x64_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_win64.zip"
  let get_x86_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_win32.zip"
  let get_linux_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_linux64.zip"
  let get_mac_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_mac64.zip"
  let get_arm_driver_url version = sprintf $"https://msedgedriver.azureedge.net/%s{version}/edgedriver_arm64.zip"

