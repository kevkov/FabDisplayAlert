namespace FabDisplayAlert

open Fabulous
open Xamarin.Forms
open Fabulous.XamarinForms

open type View

module App =
    type Model = { Count: int }

    type Msg =
        | Display1
        | Display2
        | Display3
        | Display4
        | Display5
        | Display6
        | Display7
        | AlertResult of bool

    let init () = { Count = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Display1 ->
            Application.Current.MainPage.DisplayAlert("Game over", "Yup1", "OK")
            |> ignore

            model, Cmd.none

        | Display2 ->
            Application.Current.Dispatcher.BeginInvokeOnMainThread (fun () ->
                Application.Current.MainPage.DisplayAlert("Game over", "Yup2", "OK")
                |> ignore)

            model, Cmd.none

        | Display3 ->
            async {
                do!
                    Application.Current.MainPage.DisplayAlert("Game over", "Yup1", "OK")
                    |> Async.AwaitTask
            }
            |> Async.StartImmediate

            model, Cmd.none

        | Display4 ->
            Application.Current.Dispatcher.BeginInvokeOnMainThread (fun () ->
                async {
                    do!
                        Application.Current.MainPage.DisplayAlert("Game over", "Yup1", "OK")
                        |> Async.AwaitTask
                }
                |> Async.StartImmediate)

            model, Cmd.none

        | Display5 ->
            let alertResult =
                async {
                    let! alert =
                        Application.Current.MainPage.DisplayAlert("Display Alert", "Confirm", "Ok", "Cancel")
                        |> Async.AwaitTask

                    return AlertResult alert
                }

            model, Cmd.ofAsyncMsg alertResult

        | Display6 ->
            let alertResult =
                Device.InvokeOnMainThreadAsync (funcTask = fun () ->
                    task {
                        let! alert =
                            Application.Current.MainPage.DisplayAlert("Display Alert", "Confirm", "Ok", "Cancel")

                        return AlertResult alert
                    }) 

            model, Cmd.ofAsyncMsg (async { return! (alertResult |> Async.AwaitTask) })

        | AlertResult value ->
            System.Console.WriteLine $"AlertResult is {value}"
            model, Cmd.none
        

    let view model =
        Application(
            ContentPage(
                "FabDisplayAlert",
                VStack() {
                    Label("Hello from Fabulous v2!")
                        .font(namedSize = NamedSize.Title)
                        .centerTextHorizontal ()

                    (VStack() {
                        Label($"Count is {model.Count}")
                            .centerTextHorizontal ()

                        Button("Display1", Display1)
                        Button("Display2", Display2)
                        Button("Display3", Display3)
                        Button("Display4", Display4)
                        Button("Display5", Display5)
                        Button("Display6", Display6)
                    })
                        .centerVertical (expand = true)
                }
            )
        )

    let program =
        Program.statefulWithCmd init update view
