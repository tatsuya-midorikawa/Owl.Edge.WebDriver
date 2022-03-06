// ▼ Microsoft Edge Driver
// https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/

// ▼ Selenium WebDriver Documentation | Selenium
// https://www.selenium.dev/documentation/webdriver/

// ▼ Selenuium | GitHub
// https://github.com/SeleniumHQ/selenium/tree/trunk/dotnet/src/webdriver

namespace Owl.Edge.WebDriver

open System
open System.IO
open OpenQA.Selenium
open OpenQA.Selenium.Edge
open Trunk

[<AutoOpen>]
module Edge =

  /// <summary>
  /// </summary>
  type EdgeOptionsBuilder () =
    let option = EdgeOptions(BinaryLocation = stable.FullName)
    member __.Yield(_) = option
    member __.Zero() = option

    /// <summary>
    /// 利用する msedge.exe を直接指定する.
    /// </summary>
    [<CustomOperation("binary")>]
    member __.BinaryLocation (options: EdgeOptions, location: string) =
      options.BinaryLocation <- location
      options
    
    /// <summary>
    /// 既定のインストール先にインストールされている, Stable チャネルの msedge.exe を利用する.
    /// </summary>
    [<CustomOperation("use_stable")>]
    member __.UseStable (options: EdgeOptions) =
      options.BinaryLocation <- stable.FullName
      options
   
    /// <summary>
    /// 既定のインストール先にインストールされている, Beta チャネルの msedge.exe を利用する.
    /// </summary>
    [<CustomOperation("use_beta")>]
    member __.UseBeta (options: EdgeOptions) =
      options.BinaryLocation <- beta.FullName
      options
      
    /// <summary>
    /// 既定のインストール先にインストールされている, Dev チャネルの msedge.exe を利用する.
    /// </summary>
    [<CustomOperation("use_dev")>]
    member __.UseDev (options: EdgeOptions) =
      options.BinaryLocation <- dev.FullName
      options
      
    /// <summary>
    /// 既定のインストール先にインストールされている, Cahary チャネルの msedge.exe を利用する.
    /// </summary>
    [<CustomOperation("use_canary")>]
    member __.UseCanary (options: EdgeOptions) =
      options.BinaryLocation <- canary.FullName
      options
      
    /// <summary>
    /// 指定した URL を app-mode で起動する.
    /// </summary>
    [<CustomOperation("use_app_mode")>]
    member __.AppMode (options: EdgeOptions, url: string) =
      options.AddArgument $"app=%s{url}"
      options
      
    /// <summary>
    /// msedge.exe を headless-mode で起動する.
    /// </summary>
    [<CustomOperation("use_headless")>]
    member __.Headless (options: EdgeOptions) =
      options.AddArgument $"headless"
      options

    /// <summary>
    /// Adds arguments to be appended to the browser executable command line.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="argument"></param>
    /// <see href="https://peter.sh/experiments/chromium-command-line-switches/">
    /// List of Chromium Command Line Switches
    /// </see>
    [<CustomOperation("args")>]
    member __.AddArgument (options: EdgeOptions, [<System.ParamArray>]arguments: string[]) =
      options.AddArguments arguments
      options
      
    /// <summary>
    /// Provides a means to add additional capabilities not yet added as type safe options for the Edge driver.
    /// </summary>
    /// <see href="https://github.com/SeleniumHQ/selenium/blob/c3a19c8bcbdc826443c14e0fa4d1361eaed72fff/dotnet/src/webdriver/Edge/EdgeOptions.cs#L101">
    /// Source code.
    /// </see>
    [<CustomOperation("capability")>]
    member __.AddAdditionalEdgeOption (options: EdgeOptions, name: string, value: string) =
      options.AddAdditionalEdgeOption(name, value)
      options


      
  /// <summary>
  /// </summary>
  type EdgeDriverBuilder (options: EdgeOptions, ?driverDir: string, ?url: string) =
    let driver = 
      match (driverDir, url) with
      | (Some dir, Some url) -> new EdgeDriver(dir, options, Url= url)
      | (Some dir, None) -> new EdgeDriver(dir, options)
      | (None, Some url) -> new EdgeDriver(options, Url= url)
      | (None, None) -> new EdgeDriver(options)
    let navigator = driver.Navigate()

    member __.Yield(_) = driver
    member __.For(e, f) = f e
    member __.Zero() = driver
    member __.Return(e) = e
    member __.Bind(e, f) = f e
    
    [<CustomOperation("url")>]
    member __.Url (driver: EdgeDriver, url: string) = driver.Url <- url; driver

    [<CustomOperation("navigate")>]
    member __.Navigate (driver: EdgeDriver, url: string) = navigator.GoToUrl(url); driver

    [<CustomOperation("back")>]
    member __.Back (driver: EdgeDriver) = navigator |> back |> ignore; driver

    [<CustomOperation("forward")>]
    member __.Forward (driver: EdgeDriver) = navigator |> forward |> ignore; driver

    [<CustomOperation("refresh")>]
    member __.Refresh (driver: EdgeDriver) = navigator |> refresh |> ignore; driver

    [<CustomOperation("close")>]
    member __.Close (driver: EdgeDriver) = navigator |> refresh |> ignore; driver

    /// <summary>
    /// https://developer.mozilla.org/ja/docs/Web/API/Document/getElementById
    /// </summary>
    [<CustomOperation("get_element_by_id", AllowIntoPattern=true)>]
    member __.FindElement (driver: EdgeDriver, id: string) = 
      driver.FindElement (By.Id id)

    /// <summary>
    /// https://developer.mozilla.org/ja/docs/Web/API/Document/getElementsByClassName
    /// </summary>
    [<CustomOperation("get_elements_by_class_name", AllowIntoPattern=true)>]
    member __.FindElementsByClassName (driver: EdgeDriver, class'name: string) = 
      driver.FindElements (By.ClassName class'name)
    
    /// <summary>
    /// https://developer.mozilla.org/ja/docs/Web/API/Document/getElementsByName
    /// </summary>
    [<CustomOperation("get_elements_by_name", AllowIntoPattern=true)>]
    member __.FindElementsByName (driver: EdgeDriver, name: string) = 
      driver.FindElements (By.Name name)
    
    /// <summary>
    /// https://developer.mozilla.org/ja/docs/Web/API/Document/getElementsByTagName
    /// </summary>
    [<CustomOperation("get_elements_by_tag_name", AllowIntoPattern=true)>]
    member __.FindElementsByTagName (driver: EdgeDriver, tag'name: string) = 
      driver.FindElements (By.TagName tag'name)

    /// <summary>
    /// </summary>
    [<CustomOperation("title", AllowIntoPattern=true)>]
    member __.GetTitle (driver: EdgeDriver) = driver.Title

    /// <summary>
    /// </summary>
    [<CustomOperation("page_source", AllowIntoPattern=true)>]
    member __.GetPageSource (driver: EdgeDriver) = driver.PageSource

    /// <summary>
    /// Executes JavaScript in the context of the currently selected frame or window.
    /// </summary>
    [<CustomOperation("execute", AllowIntoPattern=true)>]
    member __.Execute (driver: EdgeDriver, script: string, [<ParamArray>] arguments: string[]) = driver.ExecuteScript(script, arguments)

    /// <summary>
    /// Executes JavaScript in the context of the currently selected frame or window.
    /// </summary>
    [<CustomOperation("execute", AllowIntoPattern=true)>]
    member __.Execute (driver: EdgeDriver, script: PinnedScript, arguments: string[]) = driver.ExecuteScript(script, arguments)

    /// <summary>
    /// Executes JavaScript "asynchronously" in the context of the currently selected frame or window,
    /// executing the callback function specified as the last argument in the list of arguments.
    /// </summary>
    [<CustomOperation("execute_async", AllowIntoPattern=true)>]
    member __.ExecuteAsync (driver: EdgeDriver, script: string, [<ParamArray>] arguments: string[]) = driver.ExecuteAsyncScript(script, arguments)

    member private __.Test (driver: EdgeDriver, id: string) = 
      driver.Quit()
      let e = driver.FindElement (By.Id id)
      ()

      

  /// <summary>
  /// </summary>
  type WebElementBuilder (element: IWebElement) =
    member __.Yield(e) = element
    member __.For(e, f) = f e
    member __.Zero() = ()
    member __.Return(e) = e
    member __.Bind(e, f) = f e
    
    /// <summary>
    /// Clicks this element.
    /// </summary>
    [<CustomOperation("click")>]
    member __.Click (element: IWebElement) = element |> click; element

    /// <summary>
    /// Simulates typing text into the element.
    /// </summary>
    [<CustomOperation("input")>]
    member __.Input (element: IWebElement, value: string) = element |> input value
    
    /// <summary>
    /// Clears the content of this element.
    /// </summary>
    [<CustomOperation("clear")>]
    member __.Clear (element: IWebElement) = element |> clear
    
    /// <summary>
    /// Submits this element to the web server.
    /// </summary>
    [<CustomOperation("submit")>]
    member __.Submit (element: IWebElement) = element |> submit; element

    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_element_by_id", AllowIntoPattern=true)>]
    member __.FindElement (element: IWebElement, id: string) = element.FindElement (By.Id id)
    
    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_elements_by_name", AllowIntoPattern=true)>]
    member __.FindElements (element: IWebElement, name: string) = element.FindElements (By.Name name)

    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_elements_by_class_name", AllowIntoPattern=true)>]
    member __.FindElementsByClassName (element: IWebElement, class'name: string) = element.FindElements (By.ClassName class'name)
    
    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_elements_by_tag_name", AllowIntoPattern=true)>]
    member __.FindElementsByTagName (element: IWebElement, tag'name: string) = element.FindElements (By.TagName tag'name)
    
    /// <summary>
    /// Gets the innerText of this element, without any leading or trailing whitespace, and with other whitespace collapsed.
    /// </summary>
    [<CustomOperation("text", AllowIntoPattern=true)>]
    member __.GetText (element: IWebElement) = element |> get'text
      



  /// <summary>
  /// </summary>
  type OptionWebElementBuilder (element: option<IWebElement>) =
    member __.Yield(e) = element
    member __.For(e, f) = f e
    member __.Zero() = ()
    member __.Return(e) = e
    member __.Bind(e, f) = f e
    
    /// <summary>
    /// Clicks this element.
    /// </summary>
    [<CustomOperation("click")>]
    member __.Click (element: option<IWebElement>) = element |> Option.iter click; element

    /// <summary>
    /// Simulates typing text into the element.
    /// </summary>
    [<CustomOperation("input")>]
    member __.Input (element: option<IWebElement>, value: string) = element |> Option.map (input value)
    
    /// <summary>
    /// Clears the content of this element.
    /// </summary>
    [<CustomOperation("clear")>]
    member __.Clear (element: option<IWebElement>) = element |> Option.map clear
    
    /// <summary>
    /// Submits this element to the web server.
    /// </summary>
    [<CustomOperation("submit")>]
    member __.Submit (element: option<IWebElement>) = element |> Option.iter submit; element

    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_element_by_id", AllowIntoPattern=true)>]
    member __.FindElement (element: option<IWebElement>, id: string) = element |> Option.map (fun e -> e.FindElement (By.Id id))
    
    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_elements_by_name", AllowIntoPattern=true)>]
    member __.FindElements (element: option<IWebElement>, name: string) = element |> Option.map (fun e -> e.FindElements (By.Name name))

    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_elements_by_class_name", AllowIntoPattern=true)>]
    member __.FindElementsByClassName (element: option<IWebElement>, class'name: string) = element |> Option.map (fun e -> e.FindElements (By.ClassName class'name))
    
    /// <summary>
    /// Finds all child elements matching the given mechanism and value.
    /// </summary>
    [<CustomOperation("get_elements_by_tag_name", AllowIntoPattern=true)>]
    member __.FindElementsByTagName (element: option<IWebElement>, tag'name: string) = element |> Option.map (fun e -> e.FindElements (By.TagName tag'name))
    
    /// <summary>
    /// Gets the innerText of this element, without any leading or trailing whitespace, and with other whitespace collapsed.
    /// </summary>
    [<CustomOperation("text", AllowIntoPattern=true)>]
    member __.GetText (element: option<IWebElement>) = element |> Option.map get'text
      
  

  let options = EdgeOptionsBuilder()
  let inline edge options = EdgeDriverBuilder options
  let inline edge' options driverDirectory url = EdgeDriverBuilder (options, driverDir= driverDirectory,url= url)
  let inline edge'with options driverDirectory = EdgeDriverBuilder (options, driverDir= driverDirectory)
  let inline edge'at options url = EdgeDriverBuilder (options, url= url)
  let inline element e = WebElementBuilder e
  let inline try'element e = OptionWebElementBuilder e