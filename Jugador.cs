using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProyectoJuego
{
    // Clase que representa al jugador en el juego
    public class Jugador
    {
        private float speed;
        private int vida;
        private Rectangle bounds;
        private Texture2D bulletTexture;

        public List<Projectile> balas { get; private set; }
        public Texture2D texture { get; private set; }
        public Vector2 position;
        public int Vida => vida;

        // add this to a component class
        public SoundEffect sfx_disparo;
        public SoundEffectInstance sfx_daño_enemigo;
        public SoundEffectInstance sfx_muerto;


        public Jugador(Texture2D texture, Texture2D bulletTexture, Vector2 position, float speed,Rectangle bounds, int vida)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;
            this.position = position;
            this.speed = speed;
            this.vida = vida;
            this.bounds = bounds;
            balas = new();
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyDown(Keys.W) ||
                keyboardState.IsKeyDown(Keys.Up))
            { position.Y -= speed; }
            if (keyboardState.IsKeyDown(Keys.S) ||
                keyboardState.IsKeyDown(Keys.Down))
            { position.Y += speed; }

            position.Y = MathHelper.Clamp(position.Y, bounds.Top, bounds.Bottom - texture.Height); // clamp to max height

            if ((keyboardState.IsKeyDown(Keys.A) ||
                keyboardState.IsKeyDown(Keys.Left)))
            { position.X -= speed; }
            if ((keyboardState.IsKeyDown(Keys.D) ||
                keyboardState.IsKeyDown(Keys.Right)))
            { position.X += speed; }

            position.X = MathHelper.Clamp(position.X, bounds.Left, bounds.Right - texture.Width); // clamp to max width

            if (keyboardState.IsKeyDown(Keys.Space))
            { Disparar(); }

            for (int i = balas.Count - 1; i >= 0; i--)
            {
                balas[i].Update(gameTime);
                if (balas[i].position.Y < 0)
                { balas.RemoveAt(i); }
            }
            Console.Clear();
            Console.WriteLine($"jugador_pos: {position}");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            foreach (var bala in balas)
            { bala.Draw(spriteBatch); }
        }

        private void Disparar()
        {
          sfx_disparo.Play();
          Vector2 bulletPositionLeft = new (
            position.X - bulletTexture.Width/30,
            position.Y + texture.Height - bulletTexture.Height
          );
          Projectile balaIzquierda = new(
            bulletTexture, ProjectileType.PlayerShoot, 
            bulletPositionLeft, new(0,1), 5f
          );
          balas.Add(balaIzquierda);
        }
        public void ReducirVida(int cantidad)
        {
            if (vida <= 0)
            {
              sfx_muerto?.Play();
              return;
            }
            vida -= cantidad;
        }
        public void DrawHealthBar(SpriteBatch spriteBatch, Texture2D texture)
        {
            int barWidth = 100;
            int barHeight = 50;
            int healthWidth = (int)((vida / 10f) * barWidth);
            spriteBatch.Draw( //drawing background
                texture,
                new Rectangle((int)position.X, (int)(position.Y - barHeight*1.2f), barWidth, barHeight),
                Color.Gray
            );
            spriteBatch.Draw( //drawing foreground
                texture,
                new Rectangle((int)position.X, (int)(position.Y - barHeight*1.2f), healthWidth, barHeight),
                Color.Green
            );
        }
    }
}
