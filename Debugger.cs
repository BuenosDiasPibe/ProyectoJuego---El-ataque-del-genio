/*
    From Juegazo codebase
*/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego
{
  public class Debugger
  {
        private Texture2D rectangleTexture;
        private static object sync = new();
        private static Debugger instance;
        public bool canDraw = false;
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (instance != null) return;
            lock (sync)
            {
                if (instance == null)
                {
                    instance = new Debugger(graphicsDevice);
                }
            }
        }
        public static Debugger Instance
        {
            get
            {
                if (instance == null) throw new Exception("Debugger not initialized");
                return instance;
            }
        }
        private Debugger(GraphicsDevice graphicsDevice)
        {
            rectangleTexture = new Texture2D(graphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 255, 255, 255) });
        }
        public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
        {
          if(!canDraw) return;
          spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
              rect.X,
              rect.Y,
              rect.Width,
              thickness),
            color
          );
          spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
              rect.X,
              rect.Bottom - thickness,
              rect.Width,
              thickness),
            color
          );
          spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.X,
                rect.Y,
                thickness,
                rect.Height),
            color
          );
          spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
            rect.Right - thickness,
            rect.Y,
            thickness,
            rect.Height),
            color
          );
        }
  }
}
