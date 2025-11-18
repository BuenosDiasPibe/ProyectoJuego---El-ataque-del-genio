using System;
using System.Diagnostics.Tracing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProyectoJuego
{
    // Clase que gestiona el Menú Principal del juego
    public class MainMenu : Scene
    {
        private Texture2D buttonTexture;
        private SpriteFont font;
        private Rectangle playButton;
        private Rectangle exitButton;
        private Action startGameAction;
        private Action exitAction;
        //private Action stupidAction;
        private Texture2D backgroundTexture;

        private SceneManager sceneManager;

        public MainMenu(Texture2D buttonTexture, SpriteFont font, Texture2D backgroundTexture, Action startGameAction, Action exitAction, Action stupidAction)
        {
            this.buttonTexture = buttonTexture;
            this.font = font;
            this.backgroundTexture = backgroundTexture;
            this.startGameAction = startGameAction;
            this.exitAction = exitAction;

            playButton = new Rectangle(525, 550, 200, 50);
            exitButton = new Rectangle(525, 650, 200, 50);
        }
        public MainMenu(SceneManager sceneManager)
        {
          MediaPlayer.Pause(); // pause whatever song was playing before this class was created
          this.sceneManager = sceneManager;
          this.content = sceneManager.contentManager;
          this.graphics = sceneManager.graphics;
          buttonTexture = content.Load<Texture2D>("button");
          font = content.Load<SpriteFont>("font");
          backgroundTexture = content.Load<Texture2D>("menu");
          exitAction = sceneManager.actionByState[GameState.Exit];
          startGameAction = sceneManager.actionByState[GameState.Playing];

          playButton = new Rectangle(525, 550, 200, 50);
          exitButton = new Rectangle(525, 650, 200, 50);
        }


        public override void Update(GameTime gameTime)
        {
          MouseState mouseState = Mouse.GetState();

          if (mouseState.LeftButton == ButtonState.Pressed)
          {
              if (playButton.Contains(mouseState.Position))
              {
                  startGameAction?.Invoke();
              }
              else if (exitButton.Contains(mouseState.Position))
              {
                  exitAction?.Invoke();
              }
          }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Dibuja la textura de fondo del menú principal
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 1000), Color.White);

            // Dibuja los botones en la pantalla con su respectiva textura
            spriteBatch.Draw(buttonTexture, playButton, Color.White); // Dibuja el botón "Jugar"
            spriteBatch.Draw(buttonTexture, exitButton, Color.White); // Dibuja el botón "Salir"

            // Posiciones para el texto "Jugar" y "Salir" dentro de los botones
            Vector2 playTextPosition = new Vector2(playButton.X + 95, playButton.Y + 15);
            Vector2 exitTextPosition = new Vector2(exitButton.X + 95, exitButton.Y + 15);

            try
            {
                // Dibuja el texto "Jugar" y "Salir" en los botones
                spriteBatch.DrawString(font, "Jugar", playTextPosition, Color.Black);
                spriteBatch.DrawString(font, "Salir", exitTextPosition, Color.Black);
            }
            catch (ArgumentException)
            {
                // Si ocurre una excepción, usa texto en mayúsculas como alternativa
                spriteBatch.DrawString(font, "JUGAR", playTextPosition, Color.Black);
                spriteBatch.DrawString(font, "SALIR", exitTextPosition, Color.Black);
            }
        }

        public override void LoadContent()
        { }

        public override void UnloadContent()
        { }

        public override void DrawUI(GameTime gameTime, SpriteBatch spriteBatch)
        { }
    }
}
