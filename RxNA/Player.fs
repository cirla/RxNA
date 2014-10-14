module RxNA.Player

open System

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open RxNA.Renderer

type Player =
    { position: Vector2;
      velocity: Vector2;
      maxVelocity: float32;
      spritesheet: string;
      frame: int;
      frameDimensions: Vector2 }

let playerRender player res =
    let texture = res.textures.Item player.spritesheet
    let spritesPerRow = texture.Bounds.Width / (int player.frameDimensions.X)
    let xOffset = player.frame % spritesPerRow
    let yOffset = player.frame / spritesPerRow
    let sourceRect =
        new Rectangle(xOffset * (int player.frameDimensions.X),
                      yOffset * (int player.frameDimensions.Y),
                      int player.frameDimensions.X,
                      int player.frameDimensions.Y)

    res.spriteBatch.Draw(texture,
                         player.position,
                         new Nullable<Rectangle>(sourceRect),
                         Color.White)

// Creating a single player here for now
let mutable player =
    { Player.position = Vector2.Zero;
      velocity = Vector2.Zero;
      maxVelocity = 100.0f;
      spritesheet = "player_spritesheet";
      frame = 2;
      frameDimensions = new Vector2(72.0f, 96.0f)}

let playerInit =
    Renderer.renderStream
    |> Observable.subscribe (playerRender player)
