module RxNA.Renderer

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open System.Reactive.Subjects

type TextureMap = Map<string, Texture2D>

type RenderResources = {
    graphics: GraphicsDevice;
    spriteBatch: SpriteBatch;
    textures: TextureMap }

let renderStream =
    new Subject<RenderResources>()
 
let render (res:RenderResources) =
    res.graphics.Clear(Color.Black)

    res.spriteBatch.Begin()
    renderStream.OnNext(res)
    res.spriteBatch.End()
