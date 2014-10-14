module RxNA.Player

open System

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open RxNA.Input
open RxNA.Renderer

type Player =
    { position: Vector2;
      velocity: Vector2;
      maxVelocity: float32;
      spritesheet: string;
      frame: int;
      frameDimensions: Vector2 }

let updateVelocity (velocity:Vector2) maxVelocity = function
    | Keys.W -> new Vector2(velocity.X, -maxVelocity)
    | Keys.S -> new Vector2(velocity.X, maxVelocity)
    | Keys.A -> new Vector2(-maxVelocity, velocity.Y)
    | Keys.D -> new Vector2(maxVelocity, velocity.Y)
    | _ -> velocity

let playerMove player key stop =
    let maxVelocity = match stop with
                      | false -> player.maxVelocity
                      | true -> 0.0f
    { player with
             velocity = updateVelocity player.velocity maxVelocity key }

let playerUpdate player (elapsedTime:TimeSpan) =
    { player with
             position = new Vector2(player.position.X + player.velocity.X * (float32 elapsedTime.TotalSeconds),
                                    player.position.Y + player.velocity.Y * (float32 elapsedTime.TotalSeconds)) }

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
    Input.keyDownStream
    |> Observable.subscribe (fun k ->
        player <- playerMove player k false)
    |> ignore

    Input.keyUpStream
    |> Observable.subscribe (fun k ->
        player <- playerMove player k true)
    |> ignore

    Input.elapsedTimeStream
    |> Observable.subscribe (fun t ->
        player <- playerUpdate player t)
    |> ignore

    Renderer.renderStream
    |> Observable.subscribe (fun res ->
        playerRender player res)
    |> ignore
