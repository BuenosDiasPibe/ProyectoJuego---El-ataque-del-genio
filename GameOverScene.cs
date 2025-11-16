
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
namespace ProyectoJuego 
{
    public class GameOverScene : Scene
    {
      SpriteFont font;
      private event Action reset;
      private event Action Exit;
      public GameOverScene(SceneManager sceneManager)
      {
        this.graphics = sceneManager.graphics;
        this.content = sceneManager.contentManager;
        reset = sceneManager.actionByState[GameState.Reset];
        Exit = sceneManager.actionByState[GameState.Exit];
      }
      public override void LoadContent()
      {
        font = content.Load<SpriteFont>("font");
      }

      public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
        spriteBatch.DrawString(font, "Game Over",
          new Vector2(graphics.PreferredBackBufferWidth / 2 - 100,
                      graphics.PreferredBackBufferHeight / 2 - 50),
          Color.Red);
        spriteBatch.DrawString(font, "Presiona R para reiniciar o Esc para salir",
          new Vector2(graphics.PreferredBackBufferWidth / 2 - 200,
                      graphics.PreferredBackBufferHeight / 2 + 50),
          Color.White);
      }

      public override void DrawUI(GameTime gameTime, SpriteBatch spriteBatch)
      { }


      public override void UnloadContent()
      { }

      public override void Update(GameTime gameTime)
      {
        var keyboardStateGameOver = Keyboard.GetState();
        if (keyboardStateGameOver.IsKeyDown(Keys.R))
        { reset?.Invoke(); }
        else if (keyboardStateGameOver.IsKeyDown(Keys.Escape))
        { Exit?.Invoke(); }
      }
  }
}
