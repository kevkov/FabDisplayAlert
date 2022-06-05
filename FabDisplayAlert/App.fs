namespace FabDisplayAlert

open Fabulous
open Xamarin.Forms
open Fabulous.XamarinForms

open type View

module App =
    type Model = { Count: int }

    type Msg =
        | Alert1
        | Alert2
        | Alert3
        | Alert4
        | Alert5
        | Alert6
        | Alert7
        | AlertResult of bool

    let init () = { Count = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Alert1 ->
            Application.Current.MainPage.DisplayAlert("Alert 1", "Yup", "OK")
            |> ignore

            model, Cmd.none

        | Alert2 ->
            Application.Current.Dispatcher.BeginInvokeOnMainThread (fun () ->
                Application.Current.MainPage.DisplayAlert("Alert 2", "Yup", "OK")
                |> ignore)

            model, Cmd.none

        | Alert3 ->
            async {
                do!
                    Application.Current.MainPage.DisplayAlert("Alert 3", "Yup3", "OK")
                    |> Async.AwaitTask
            }
            |> Async.StartImmediate

            model, Cmd.none

        | Alert4 ->
            Application.Current.Dispatcher.BeginInvokeOnMainThread (fun () ->
                async {
                    do!
                        Application.Current.MainPage.DisplayAlert("Alert 4", "Yup", "OK")
                        |> Async.AwaitTask
                }
                |> Async.StartImmediate)

            model, Cmd.none

        | Alert5 ->
            let alertResult =
                async {
                    let! alert =
                        Application.Current.MainPage.DisplayAlert("Alert 5", "Confirm", "Ok", "Cancel")
                        |> Async.AwaitTask

                    return AlertResult alert
                }

            model, Cmd.ofAsyncMsg alertResult

        | Alert6 ->
            let alertResult =
                Device.InvokeOnMainThreadAsync(
                    funcTask =
                        fun () ->
                            task {
                                let! alert =
                                    Application.Current.MainPage.DisplayAlert("Alert 6", "Confirm", "Ok", "Cancel")

                                return AlertResult alert
                            }
                )

            model, Cmd.ofAsyncMsg (async { return! (alertResult |> Async.AwaitTask) })

        | Alert7 ->
            let alertResult =
                Device.InvokeOnMainThreadAsync(
                    funcTask =
                        fun () ->
                            task {
                                return!
                                    async {
                                        let! alert =
                                            Application.Current.MainPage.DisplayAlert(
                                                "Alert 7",
                                                "Confirm",
                                                "Ok",
                                                "Cancel"
                                            )
                                            |> Async.AwaitTask

                                        return AlertResult alert
                                    }
                            }
                )

            model,
            Cmd.ofAsyncMsg (
                async {
                    let! result = alertResult |> Async.AwaitTask
                    return result
                }
            )

        | AlertResult value ->
            System.Console.WriteLine $"AlertResult is {value}"
            model, Cmd.none


    let view model =
        let description desc works =
            (HStack() {
                if works then
                     Label("✅")
                         .textColor (Color.Green.ToFabColor())
                 else
                     Label("❌").textColor (Color.Red.ToFabColor())
                Label(desc)
            })
                .margin (Thickness(5., 0.))

        Application(
            ContentPage(
                "FabDisplayAlert",
                VStack() {
                    Label("Hello from Fabulous v2!")
                        .font(namedSize = NamedSize.Title)
                        .centerTextHorizontal ()

                    (VStack() {
                        description "Call DisplayAlert directly from update ignoring the Task result" false
                        Button("Alert 1", Alert1)

                        description "Call DisplayAlert directly scheduled on the Dispatcher main thread, ignoring the Task result" true
                        Button("Alert 2", Alert2)

                        description "Await a DisplayAlert task from an async computation started on the update thread" false
                        Button("Alert 3", Alert3)
                        
                        description "Await a DisplayAlert task from an async computation started on the Dispatcher main thread" true
                        Button("Alert 4", Alert4)
                        
                        description "Await a DisplayAlert task from an async computation executed by Cmd.ofAsyncMsg" false
                        Button("Alert 5", Alert5)

                        description "Await a task calling DisplayAlert that is executed on the Device main thread from Cmd.ofAsyncMsg" true
                        Button("Alert 6", Alert6)
                        
                        description "Call DisplayAlert through task/async computations on the Device main thread from Cmd.ofAsyncMsg in the same manner as FabulousContacts " false
                        Button("Alert 7", Alert7)
                    })
                        .centerVertical (expand = true)
                }
            )
        )

    let program =
        Program.statefulWithCmd init update view
