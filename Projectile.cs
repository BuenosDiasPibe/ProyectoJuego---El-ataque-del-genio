using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego
{
    public enum ProjectileType
    {
        Obstacle,
        EnemyShoot,
        PlayerShoot
    }
    public class Projectile
    {
        public Texture2D texture;
        public Vector2 position;
        private Vector2 direction;
        public ProjectileType pt { get; }
        private float speed;

        public Projectile(Texture2D texture, ProjectileType pt, Vector2 position, Vector2 direction, float speed)
        {
            this.texture = texture;
            this.pt = pt;
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
            spriteBatch.Draw(texture, position, Color.Yellow);
            Debugger.Instance.DrawRectHollow(spriteBatch, new((int)position.X, (int)position.Y, texture.Width, texture.Height), 4, Color.Red);
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D texture) //when changing to new shit
        {
            spriteBatch.Draw(texture, position, Color.Yellow);
            Debugger.Instance.DrawRectHollow(spriteBatch, new((int)position.X, (int)position.Y, texture.Width, texture.Height), 4, Color.Red);
        }
    }
}