module RxNA.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open RxNA.Input
open RxNA.Renderer

type Game () as this =
    inherit Microsoft.Xna.Framework.Game()

    let graphics = new GraphicsDeviceManager(this)

    override this.Initialize() =
        base.Initialize()

        Input.keyDownStream
        |> Observable.subscribe
            (function | Keys.Escape -> this.Exit()
                      | _ -> ())
        |> ignore

    override this.LoadContent() =
        ()

    override this.Update gameTime =
        Input.keyboardStateStream.OnNext(Keyboard.GetState())
        Input.gameTimeStream.OnNext(gameTime)

    override this.Draw (gameTime) =
        Renderer.render this.GraphicsDevice
