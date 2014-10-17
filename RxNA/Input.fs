module RxNA.Input

open System
open System.Linq

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

open System.Reactive.Subjects
open FSharp.Control.Reactive

let gameTimeStream =
    new Subject<GameTime>()

let elapsedTimeStream =
    gameTimeStream
    |> Observable.map (fun g -> g.ElapsedGameTime)

let totalTimeStream =
    gameTimeStream
    |> Observable.map (fun g -> g.TotalGameTime)

let keyboardStateStream =
    new Subject<KeyboardState>()

let keysPressedStream =
    keyboardStateStream
    |> Observable.map (fun state -> state.GetPressedKeys())

let mouseStateStream =
    new Subject<MouseState>()

type MouseButton =
    | LeftButton = 0
    | MiddleButton = 1
    | RightButton = 2
    | XButton1 = 3
    | XButton2 = 4

let mouseButtonsPressedStream =
    mouseStateStream
    |> Observable.map (fun state -> 
        [|
            (state.LeftButton, MouseButton.LeftButton);
            (state.MiddleButton, MouseButton.MiddleButton);
            (state.RightButton, MouseButton.RightButton);
            (state.XButton1, MouseButton.XButton1);
            (state.XButton2, MouseButton.XButton2);
        |]
        |> Array.choose (function
            | (ButtonState.Pressed, b) -> Some(b)
            | _ -> None))

let mousePositionStream =
    mouseStateStream
    |> Observable.map (fun state -> state.Position)

let mousePositionDeltaStream =
    mousePositionStream
    |> Observable.pairwise
    |> Observable.map
        (fun (prev, curr) -> new Vector2(float32 (curr.X - prev.X), float32 (curr.Y - prev.Y)))

let mouseScrollWheelStream =
    mouseStateStream
    |> Observable.map (fun state -> state.ScrollWheelValue)

let mouseScrollWheelDeltaStream =
    mouseScrollWheelStream
    |> Observable.pairwise
    |> Observable.map
        (fun (prev, curr) -> curr - prev)

let private downStream (stream:IObservable<'T []>) =
    stream
    |> Observable.pairwise
    |> Observable.map
        (fun (prev, curr) ->
            curr.Except prev
            |> Observable.toObservable)
    |> Observable.flatmap id

let private upStream (stream:IObservable<'T []>) =
    stream
    |> Observable.pairwise
    |> Observable.map
        (fun (prev, curr) ->
            prev.Except curr
            |> Observable.toObservable)
    |> Observable.flatmap id

let private heldStream (stream:IObservable<'T []>) =
    stream
    |> Observable.map Observable.toObservable
    |> Observable.flatmap id

let keyDownStream = downStream keysPressedStream
let keyUpStream = upStream keysPressedStream
let keyHeldStream = heldStream keysPressedStream

let mouseButtonDownStream = downStream mouseButtonsPressedStream
let mouseButtonUpStream = upStream mouseButtonsPressedStream
let mouseButtonHeldStream = heldStream mouseButtonsPressedStream

let mouseClickStream button =
    mouseButtonDownStream
    |> Observable.filter (fun b -> b.Equals(button))
    |> Observable.sampleWith <| mousePositionStream
