namespace Owl.Edge.WebDriver

open System.Linq
open OpenQA.Selenium
open OpenQA.Selenium.Edge

[<AutoOpen>]
module EdgeTools =
  // https://github.com/SeleniumHQ/selenium/blob/trunk/dotnet/src/webdriver/WebDriver.cs#L516
  let inline quit (driver: EdgeDriver) = driver.Quit()
  // https://github.com/SeleniumHQ/selenium/blob/trunk/dotnet/src/webdriver/WebDriver.cs#L340
  let inline close (driver: EdgeDriver) = driver.Close()
  let inline navigate (url: string) (driver: EdgeDriver) = 
    let navigator = driver.Navigate();
    navigator.GoToUrl(url)
    navigator
  let inline back (navigator: INavigation) = navigator.Back(); navigator
  let inline forward (navigator: INavigation) = navigator.Forward(); navigator
  let inline refresh (navigator: INavigation) = navigator.Refresh(); navigator

  let inline first (elements: seq<IWebElement>) = Seq.head elements
  let inline try'first (elements: seq<IWebElement>) = Seq.tryHead elements
  let inline first'of ([<InlineIfLambda>] predicate) (elements: seq<IWebElement>) = Seq.find predicate elements
  let inline try'first'of ([<InlineIfLambda>] predicate) (elements: seq<IWebElement>) = Seq.tryFind predicate elements
  let inline find ([<InlineIfLambda>] predicate: IWebElement -> bool) (elements: seq<IWebElement>) = elements.Where predicate
  let inline exec ([<InlineIfLambda>] execute: IWebElement -> unit) (elements: seq<IWebElement>) = elements |> Seq.iter execute

  // Clicks this element.
  let inline click (element: IWebElement) = element.Click()
  // Simulates typing text into the element.
  let inline input (text: string) (element: IWebElement) = element.SendKeys(text); element
  // Clears the content of this element.
  let inline clear (element: IWebElement) = element.Clear(); element
  // Submits this element to the web server.
  let inline submit (element: IWebElement) = element.Submit()
  // Gets the value of the specified attribute or property for this element.
  let inline get'attribute (attributeName: string) (element: IWebElement) = element.GetAttribute(attributeName)
  // Gets the value of a declared HTML attribute of this element.
  let inline get'dom'attribute (attributeName: string) (element: IWebElement) = element.GetDomAttribute(attributeName)
  // Gets the innerText of this element, without any leading or trailing whitespace, and with other whitespace collapsed.
  let inline get'text (element: IWebElement) = element.Text
  // Gets the value of a CSS property of this element.
  let inline get'css'value (propertyName: string) (element: IWebElement) = element.GetCssValue propertyName
  