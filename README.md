![img](https://raw.githubusercontent.com/tatsuya-midorikawa/Edge.WebDriver/main/assets/msedge.png)  

# Owl.Edge.WebDriver  
Microsoft Edge manipulate library for F#.  

# Get Started

```fsharp
#r "nuget: Owl.Edge.WebDriver"

open Edge.WebDriver

// 'options' computation expressions allows you to create startup options for the Microsoft Edge.
let options =
  options {
    // Owl.Edge.WebDriver uses the Stable-channel by default.
    // If you want to use another channel, call the use_xxx function.
    // The list of use_xxx is as follows:
    //   | use_stable
    //   | use_beta
    //   | use_dev
    //   | use_canary
    // The following specifies that the Beta-channel should be used.
    use_beta

    // // You can also specify it directory if you have installed Edge 
    // // in a different directory than the default.
    // binary @"D:\My Program Files\Edge\Application\msedge.exe"

    // To invoke it in app-mode, call the use_app_mode function along with URL.
    use_app_mode "https://www.fsdocjp.tech"

    // // To start in headless mode, specify use_headless.
    // use_headless

    // // You can use 'args' to specify the command line options for startup.
    // args "headless"
    // // 'args' can also be used to specify more than one option at a time.
    // args "headless" "allow-outdated-plugins" "load-extension=/path/to/unpacked_extension"
  }


// 'edge' computation expressions provides the functionality to manipulate the Microsoft Edge.
let driver =
  // 'edge' is basically used with EdgeOptions instance.
  edge options {
    // By using 'navigate', you can make page transitions.
    navigate "https://www.bing.com/"

    // You can use 'title' to get a title of this page.
    title into t
    printfn $"%s{t}"

    // You can use 'get_element_by_id' to get the specific element.
    // The functions for retrieving the other elements are as follows:
    //   | get_element_by_id
    //   | get_elements_by_class_name
    //   | get_elements_by_name
    //   | get_elements_by_tag_name
    get_element_by_id "sb_form_q" into elem

    // 'element' computation expressions provides the functionality to manipulate a IWebElement instance.
    element elem {
      // Simulates typing text into the element.
      input "test"
      
      // // Clears the content of this element.
      // clear

      // // Clicks this element.
      // click

      // // Gets the innerText of this element, without any leading or trailing whitespace, 
      // // and with other whitespace collapsed.
      // text into t

      // Submits this element to the web server.
      submit
    } |> ignore

    // You can use 'page_source' to get a html of this page.
    page_source into html
    printfn $"%s{html}"
  }

  // // If you have not set the Microsoft Edge Driver as an environment variable,
  // // you can use the 'edge'with' computation expressions by specifying the directory in which it is installed.
  // let driver_dir = @"C:\tools"
  // edge'with options driver_dir {
  //   navigate "https://www.bing.com/"
  //   get_element_by_id "sb_form_q" into elem
  //   element elem {
  //     input "test"
  //     submit
  //   } |> ignore
  //   page_source into html
  //   printfn $"%s{html}"
  // }

// EdgeDriver instace will always exit with quit when you are done using it.
driver |> quit
```