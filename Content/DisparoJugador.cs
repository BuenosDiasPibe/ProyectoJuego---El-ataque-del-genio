using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego.Content
{
    public class DisparoJugador
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 direction;
        private float speed;
        public Texture2D Texture => texture;
        public Vector2 Position => position;

        public DisparoJugador(Texture2D texture, Vector2 position, Vector2 direction, float speed)
        {
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
        }
        public void Update(GameTime gameTime)
        {
            position += direction * speed;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            Debugger.Instance.DrawRectHollow(spriteBatch, new((int)position.X, (int)position.Y, texture.Width, texture.Height), 4, Color.Green);
        }
    }
}
