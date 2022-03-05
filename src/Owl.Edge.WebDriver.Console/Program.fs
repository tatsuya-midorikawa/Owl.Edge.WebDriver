//open OpenQA.Selenium

open Owl.Edge.WebDriver


let task = Trunk.download "99.0.1150.30"
System.Threading.Tasks.Task.WaitAll task
printfn "done"



//// 既定では Stable チャネルを使います.
//let options =
//  options {
//    // Beta チャネルを使う
//    use_beta
//    // app-mode を使う
//    use_app_mode "https://www.microsoft.com"
//  }

//let driver =
//  edge options {

//    navigate "https://www.bing.com/"

//    execute "alert(arguments[0])" "sample text" into result
//    get_element_by_id "sb_form_q" into e

//    element e {
//      input "test"
//      submit
//    } |> ignore

//    page_source into html
//    printfn $"%s{html}"

//    //try'element (elements |> try'first) {
//    //  input "test"
//    //  submit
//    //}
//  }


//let driver_dir = @"C:\tools\driver"
//edge'with options driver_dir {
//  navigate "https://www.bing.com/"
//  get_element_by_id "sb_form_q" into elem
//  element elem {
//    input "test"
//    submit
//  } |> ignore
//  page_source into html
//  printfn $"%s{html}"
//}
//|> ignore

//let driver =
//  edge options {
//    navigate "https://midoliy.com"
//    navigate "https://www.fsdocjp.tech"
//    get_elements_by_tag_name "a" into links
//    let link = links |> first'of (fun link -> link.Text = "F# で開発をはじめる")
//    click link
//    // or
//    //element link { click }
//  }


//let options =
//  options {
//    binary beta
//    //use_headless
//    use_app_mode "https://www.microsoft.com"

//    //args "app=https://www.microsoft.com"
//    //args "headless"
//    //args "headless" "allow-outdated-plugins" "load-extension=/path/to/unpacked_extension"

//  }

////options.BinaryLocation |> printfn "%s"
////options.Arguments |> printfn "%A"

////let driver =
////  edge'with options "https://www.microsoft.com" {
////    navigate "https://www.fsdocjp.tech"
////    navigate "https://midoliy.com"
////    back
////    get_element_by_id "sidebar" into elem
////    elem.Text |> printfn "### %s"
////  }

//let driver =
//  edge options {
//    navigate "https://www.fsdocjp.tech"
//    navigate "https://midoliy.com"
//    back
//    //get_element_by_id "sidebar" into elem
//    //elem.Text |> printfn "### %s"
//    get_elements_by_tag_name "a" into links
//    links 
//    |> first'of (fun link -> link.Text = "F# で開発をはじめる")
//    |> click
//    //let link = links |> Seq.head
//    //link.Text |> printfn "### %s"
//  }

////driver |> quit

