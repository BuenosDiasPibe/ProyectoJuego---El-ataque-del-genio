using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace ProyectoJuego
{
  public class PauseScene : Scene
  {
    SpriteFont font;
    ContentManager Content;
    GraphicsDeviceManager _graphics;
    SceneManager sceneManager;
    KeyboardState lastKey;
    bool canContinue = false;

    public PauseScene(SceneManager sceneManager, ContentManager Content, GraphicsDeviceManager _graphics)
    {
      this.sceneManager = sceneManager;
      this.Content = Content;
      this._graphics = _graphics;
    }
    public override void LoadContent()
    {
      font = Content.Load<SpriteFont>("font");
    }
    public override void Update(GameTime gameTime)
    {
      if(Keyboard.GetState().IsKeyDown(Keys.P) && lastKey.IsKeyUp(Keys.P) && canContinue)
      {
        MediaPlayer.Resume();
        sceneManager.RemoveScene();
        return;
      }
      if(Keyboard.GetState().IsKeyUp((Keys.P)))
      {
        canContinue = true;
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.DrawString(font, "PAUSE",
        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 50,
                    _graphics.PreferredBackBufferHeight / 2 - 25),
        Color.White);
      spriteBatch.DrawString(font, "Presiona P para reanudar",
        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100,
                    _graphics.PreferredBackBufferHeight / 2 + 25),
        Color.White);
    }

    public override void DrawUI(GameTime gameTime, SpriteBatch spriteBatch)
    { }

    public override void UnloadContent()
    { }
  }
}
