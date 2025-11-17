using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego
{
    public class Enemigo
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private Texture2D fireballTexture;
        private List<Projectile> bolasDeFuego;
        private float fireballCooldown = 1;
        private float timeSinceLastShot = 0;
        private float leftBoundary;
        private float rightBoundary;
        private bool movingRight;
        private int vida;

        public bool EnemigoMuerto = false;
        public int Vida => vida;
        public Vector2 Position => position;
        public Texture2D Texture => texture;
        public bool DisparoRealizado { get; set; }

        public Enemigo(Texture2D texture, Texture2D fireballTexture, Vector2 position, float speed, float leftBoundary, float rightBoundary)
        {
            this.texture = texture;
            this.fireballTexture = fireballTexture;
            this.position = position;
            this.speed = speed;
            this.leftBoundary = leftBoundary;
            this.rightBoundary = rightBoundary;
            movingRight = true;
            bolasDeFuego = new();
            vida = 10;
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
            //State Machine
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

            for (int i = bolasDeFuego.Count -1; i >= 0; i--)
            {
                bolasDeFuego[i].Update(gameTime);
                // Comprueba colisión con el jugador
                if (ColisionaConJugador(bolasDeFuego[i], jugador))
                {
                    jugador.ReducirVida(10);
                    bolasDeFuego.RemoveAt(i);
                    continue;
                }
                else if (bolasDeFuego[i].position.Y > 1000 || bolasDeFuego[i].position.Y < 0)
                {
                    bolasDeFuego.RemoveAt(i);
                    continue;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
          spriteBatch.Draw(texture, position, Color.White);
          Debugger.Instance.DrawRectHollow(spriteBatch, new((int)position.X, (int)position.Y, texture.Width, texture.Height), 4, Color.Red);

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
            Random random = new();
            Vector2 failure = new(random.Next(-20, 20));
            direction = Vector2.Add(failure, direction);
            direction.Normalize();

            Projectile nuevaBola = new(fireballTexture, ProjectileType.EnemyShoot, position, direction, 10f);
            bolasDeFuego.Add(nuevaBola);
            DisparoRealizado = true;
        }
        private static bool ColisionaConJugador(Projectile bolaDeFuego, Jugador jugador)
        {
            //TODO: poner las coliciones en el centro de los rectangulos
            //float escalaBolaDeFuego = 0.01f;
            Rectangle balaRect = new Rectangle(
                (int)bolaDeFuego.position.X,
                (int)bolaDeFuego.position.Y,
                (int)(bolaDeFuego.texture.Width), //* escalaBolaDeFuego),
                (int)(bolaDeFuego.texture.Height) // * escalaBolaDeFuego)
            );
            Rectangle jugadorRect = new Rectangle(
                (int)jugador.Position.X,
                (int)jugador.Position.Y,
                (int)(jugador.Texture.Width), //* 0.25f),
                (int)(jugador.Texture.Height)// * 0.5f)
            );
            return balaRect.Intersects(jugadorRect);
        }
        public void DrawHealthBar(SpriteBatch spriteBatch, Texture2D texture)
        {
            int barWidth = 100;
            int barHeight = 20;
            int healthWidth = (int)(vida / 100f * barWidth);

            spriteBatch.Draw(//background
              texture,
              new Rectangle(
                (int)position.X, 
                (int)(position.Y - barHeight*1.2f), 
                barWidth, barHeight),
              Color.Yellow
            );

            spriteBatch.Draw(//foreground
              texture,
              new Rectangle(
                (int)position.X, 
                (int)(position.Y - barHeight*1.2f),
                healthWidth, barHeight),
              Color.Red
            );
        }
    }
}
