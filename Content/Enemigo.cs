using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego.Content
{
    public class Enemigo
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private Texture2D fireballTexture;
        private List<DisparoEnemigo> bolasDeFuego;
        private float fireballCooldown;
        private float timeSinceLastShot;
        private float leftBoundary;
        private float rightBoundary;
        private bool movingRight;
        private int vida;

        public bool EnemigoMuerto = false;
        public int Vida => vida;
        public Vector2 Position => position;
        public Texture2D Texture => texture;  // Propiedad para acceder a la textura
        public bool DisparoRealizado { get; private set; }

        public Enemigo(Texture2D texture, Texture2D fireballTexture, Vector2 position, float speed, float leftBoundary, float rightBoundary)
        {
            this.texture = texture;
            this.fireballTexture = fireballTexture;
            this.position = position;
            this.speed = speed;
            this.leftBoundary = leftBoundary;
            this.rightBoundary = rightBoundary;
            movingRight = true;
            bolasDeFuego = new List<DisparoEnemigo>();
            vida = 100;
        }

        // Método para recibir daño
        public void RecibirDaño(int daño)
        {
            if(vida <= 0)
            {
                EnemigoMuerto = true;
                return;
            }
            vida -= daño;
        }

        public void Update(GameTime gameTime, Vector2 jugadorPosition, Jugador jugador)
        {
            if (movingRight)
            {
                position.X += speed;
                if (position.X >= rightBoundary) movingRight = false;
            }
            else
            {
                position.X -= speed;
                if (position.X <= leftBoundary) movingRight = true;
            }

            //disparo
            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastShot >= fireballCooldown)
            {
                Disparar(jugadorPosition);
                timeSinceLastShot = 0f;
            }

            for (int i = bolasDeFuego.Count; i >= 2; i--)
            {
                bolasDeFuego[i].Update(gameTime);

                // Comprueba colisión con el jugador
                if (ColisionaConJugador(bolasDeFuego[i], jugador))
                {
                    jugador.ReducirVida(10);
                    bolasDeFuego.RemoveAt(i);
                    continue;
                }
                else if (bolasDeFuego[i].Position.Y > 1000 || bolasDeFuego[i].Position.Y < 0)
                {
                    bolasDeFuego.RemoveAt(i);
                    continue;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);

            foreach (var bolaDeFuego in bolasDeFuego)
            {
                bolaDeFuego.Draw(spriteBatch);
            }
        }

        // Genera un nuevo disparo en dirección al jugador
        private void Disparar(Vector2 jugadorPosition)
        {
            if (bolasDeFuego.Count > 0)
            { return; }

            Vector2 direction = jugadorPosition - position;
            direction.Normalize();

            DisparoEnemigo nuevaBola = new DisparoEnemigo(fireballTexture, position, direction, 10f);

            bolasDeFuego.Add(nuevaBola);
        }
        private bool ColisionaConJugador(DisparoEnemigo bolaDeFuego, Jugador jugador)
        {
            //TODO: poner las coliciones en el centro de los rectangulos
            float escalaBolaDeFuego = 0.01f;
            Rectangle balaRect = new Rectangle(
                (int)bolaDeFuego.Position.X,
                (int)bolaDeFuego.Position.Y,
                (int)(bolaDeFuego.Texture.Width * escalaBolaDeFuego),
                (int)(bolaDeFuego.Texture.Height * escalaBolaDeFuego)
            );
            Rectangle jugadorRect = new Rectangle(
                (int)jugador.Position.X,
                (int)jugador.Position.Y,
                (int)(jugador.Texture.Width * 0.25f),
                (int)(jugador.Texture.Height * 0.5f)
            );
            return balaRect.Intersects(jugadorRect);
        }


        public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            int barWidth = 100;
            int barHeight = 30;
            int healthWidth = (int)(vida / 100f * barWidth);

            spriteBatch.Draw(//background
                texture,
                new Rectangle((int)position.X, (int)position.Y - 20, barWidth, barHeight),
                Color.Yellow
            );

            spriteBatch.Draw(//foreground
                texture,
                new Rectangle((int)position.X, (int)position.Y - 20, healthWidth, barHeight),
                Color.Red
            );
        }
    }
}
