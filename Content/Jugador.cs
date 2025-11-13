using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProyectoJuego.Content
{
    // Clase que representa al jugador en el juego
    public class Jugador
    {
        // Campos privados
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private int vida;
        private int screenWidth;
        private int screenHeight;
        private Texture2D bulletTexture;
        private List<DisparoJugador> balas;
        public List<DisparoJugador> Balas => balas;
        public Texture2D Texture => texture;  // Textura del jugador
        public Vector2 Position => position;  // Posición del jugador
        public int Vida => vida;              // Vida del jugador

        // Constructor
        public Jugador(Texture2D texture, Texture2D bulletTexture, Vector2 position, float speed, int screenWidth, int screenHeight, int vida)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;
            this.position = position;
            this.speed = speed;
            this.vida = vida;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            balas = new List<DisparoJugador>();
        }
        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState(); // Obtiene el estado actual del teclado
            
            // Movimiento hacia arriba
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                position.Y -= speed;

            // Movimiento hacia abajo
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                position.Y += speed;

            position.Y = MathHelper.Clamp(position.Y, 0, screenHeight - texture.Height);

            if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)) && position.X > 50)
                position.X -= speed;

            if ((keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)) && position.X < 900)
                position.X += speed;

            position.X = MathHelper.Clamp(position.X, 0, screenWidth - texture.Width);

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                Disparar();
            }

            for (int i = balas.Count - 1; i >= 0; i--)
            {
                balas[i].Update(gameTime);
                if (balas[i].Position.Y < 0)
                { balas.RemoveAt(i); }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            foreach (var bala in balas)
            { bala.Draw(spriteBatch); }
        }

        private void Disparar()
        {
            Vector2 bulletPositionLeft = new Vector2(
                position.X - bulletTexture.Width/30, // A la izquierda del jugador
                position.Y + texture.Height  - bulletTexture.Height  // Centrado verticalmente
            );
            Vector2 direction = new Vector2(0, 1);
            DisparoJugador balaIzquierda = new DisparoJugador(bulletTexture, bulletPositionLeft, direction, 5f);
            balas.Add(balaIzquierda);
        }
        public void ReducirVida(int cantidad)
        {
            if (vida <= 0) return;
            vida -= cantidad;
        }
        public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            int barWidth = 100;
            int barHeight = 50;
            int healthWidth = (int)((vida / 10f) * barWidth);
            spriteBatch.Draw( //drawing background
                texture,
                new Rectangle((int)position.X, (int)position.Y - 20, barWidth, barHeight),
                Color.Gray
            );
            spriteBatch.Draw( //drawing foreground
                texture,
                new Rectangle((int)position.X, (int)position.Y - 20, healthWidth, barHeight),
                Color.Yellow
            );
        }
    }
}
