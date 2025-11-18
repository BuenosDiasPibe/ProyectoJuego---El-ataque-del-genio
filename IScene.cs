using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego
{
  public abstract class Scene
  {
    // TODO: get back to a interface IScene, i dont need this
    public ContentManager content;
    public GraphicsDeviceManager graphics;

    public abstract void LoadContent();
    public abstract void UnloadContent();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public abstract void DrawUI(GameTime gameTime, SpriteBatch spriteBatch);
  }
}
