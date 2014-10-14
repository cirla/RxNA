﻿module RxNA.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open RxNA.Input
open RxNA.Player
open RxNA.Renderer

type Game () as this =
    inherit Microsoft.Xna.Framework.Game()

    let graphics = new GraphicsDeviceManager(this)
    let contentManager = new ContentManager(this.Services, "Content")
    let mutable renderResources =
        { Renderer.RenderResources.graphics = null;
          spriteBatch = null;
          textures = Map.empty }

    override this.Initialize() =
        base.Initialize()

        Player.playerInit
        |> ignore

        Input.keyDownStream
        |> Observable.subscribe
            (function | Keys.Escape -> this.Exit()
                      | _ -> ())
        |> ignore

    override this.LoadContent() =
        renderResources <-
            { Renderer.RenderResources.graphics = this.GraphicsDevice;
              spriteBatch = new SpriteBatch(this.GraphicsDevice);
              textures = Map.empty.Add("player_spritesheet", contentManager.Load<Texture2D>("Sprites/player_spritesheet")) }

    override this.Update gameTime =
        Input.keyboardStateStream.OnNext(Keyboard.GetState())
        Input.gameTimeStream.OnNext(gameTime)

    override this.Draw (gameTime) =
        Renderer.render renderResources
