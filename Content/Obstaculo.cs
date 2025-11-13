using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoJuego.Content
{
    public class Obstaculo
    {
            private Texture2D _texture;
            private Vector2 _position;
            private float _speed;

            public Vector2 Position => _position;

            public Obstaculo(Texture2D texture, Vector2 position, float speed)
            {
                _texture = texture;
                _position = position;
                _speed = speed;
            }
            public void Update(GameTime gameTime)
            {
                _position.Y += _speed; // Mueve el obstáculo hacia abajo
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(_texture, _position, Color.White);
            }
        }

    
}
