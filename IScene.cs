using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego
{
  public abstract class Scene
  {
    public ContentManager content;
    public GraphicsDeviceManager graphics;

    public abstract void LoadContent();
    public abstract void UnloadContent();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public abstract void DrawUI(GameTime gameTime, SpriteBatch spriteBatch);
  }
}
