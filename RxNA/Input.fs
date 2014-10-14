module RxNA.Input

open System
open System.Linq

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

open System.Reactive.Linq
open System.Reactive.Subjects

let gameTimeStream =
    new Subject<GameTime>()

let elapsedTimeStream =
    gameTimeStream
    |> Observable.map (fun g -> g.ElapsedGameTime)

let totalTimeStream =
    gameTimeStream
    |> Observable.map (fun g -> g.TotalGameTime)

let private downStream (stream:IObservable<'T []>) =
    stream
    |> Observable.pairwise
    |> Observable.map
        (fun (prev, curr) ->
            curr.Except prev
            |> Observable.ToObservable)
    |> Observable.Merge

let private upStream (stream:IObservable<'T []>) =
    stream
    |> Observable.pairwise
    |> Observable.map
        (fun (prev, curr) ->
            prev.Except curr
            |> Observable.ToObservable)
    |> Observable.Merge

let private heldStream (stream:IObservable<'T []>) =
    stream
    |> Observable.map Observable.ToObservable
    |> Observable.Merge

let keyboardStateStream =
    new Subject<KeyboardState>()

let keysPressedStream =
    keyboardStateStream
    |> Observable.map (fun state -> state.GetPressedKeys())

let keyDownStream = downStream keysPressedStream

let keyUpStream = upStream keysPressedStream

let keyHeldStream = heldStream keysPressedStream
