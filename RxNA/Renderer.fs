module RxNA.Renderer

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open System.Reactive.Subjects

let renderStream =
    new Subject<GraphicsDevice>()
 
let render (graphics:GraphicsDevice) =
    graphics.Clear(Color.CornflowerBlue)
    renderStream.OnNext(graphics)
