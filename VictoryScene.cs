
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProyectoJuego
{
    public class VictoryScene : Scene
    {
      private event Action reset;
      private event Action exit;
      private SpriteFont font;
      public VictoryScene(SceneManager sceneManager)
      {
        this.content = sceneManager.contentManager;
        this.graphics = sceneManager.graphics;
        reset = sceneManager.actionByState[GameState.Reset];
        exit = sceneManager.actionByState[GameState.Exit];
      }
      public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
        spriteBatch.DrawString(font, "GANASTE!",
            new Vector2(graphics.PreferredBackBufferWidth / 2 - 100,
                        graphics.PreferredBackBufferHeight / 2 - 50),
            Color.Green);
        spriteBatch.DrawString(font, "Presiona R para reiniciar o Esc para salir",
            new Vector2(graphics.PreferredBackBufferWidth / 2 - 200,
                        graphics.PreferredBackBufferHeight / 2 + 50),
            Color.White);
      }

      public override void DrawUI(GameTime gameTime, SpriteBatch spriteBatch)
      {
      }

      public override void LoadContent()
      {
        font = content.Load<SpriteFont>("font");
      }

      public override void UnloadContent()
      {
      }

      public override void Update(GameTime gameTime)
      {
        var victoryKeyboardState = Keyboard.GetState();
        if (victoryKeyboardState.IsKeyDown(Keys.R))
        {
            reset?.Invoke();
        }
        else if (victoryKeyboardState.IsKeyDown(Keys.Escape))
        {
            exit?.Invoke();
        }
      }
    }
}
