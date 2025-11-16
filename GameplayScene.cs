using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
namespace ProyectoJuego
{
  public class GameplayScene : Scene
  {
    private Texture2D _gameBackgroundTexture;
    private Jugador jugador;
    Texture2D jugador_text;
    private Enemigo enemigo;

    private List<Projectile> _obstaculos;
    private Texture2D _obstaculoTexture;
    private float _spawnCooldown;
    private float _timeSinceLastSpawn;

    private ContentManager cm;
    private SceneManager sm;
    private GraphicsDeviceManager gd;

    private float _backgroundOffsetY = 0;
    private readonly float _backgroundSpeed = 2;

    private event Action pause;
    private event Action gameOver;
    private event Action victory;
    KeyboardState lastKey;


    public GameplayScene( ContentManager cm, SceneManager sm, GraphicsDeviceManager gd) 
    {
      this.cm = cm;
      this.sm = sm;
      this.gd = gd;
      pause = sm.actionByState[GameState.Paused];
      gameOver = sm.actionByState[GameState.GameOver];
      victory = sm.actionByState[GameState.Victory];
    }

    public override void LoadContent()
    {
      _obstaculos = new();
      _spawnCooldown = 2f;
      _timeSinceLastSpawn = 0f;

      _obstaculoTexture = cm.Load<Texture2D>("enemigo");
      Texture2D enemyTexture = cm.Load<Texture2D>("nuevoAutoPolicia");
      Texture2D fireballTexture = cm.Load<Texture2D>("balaPolicia");
      Texture2D playerTexture = cm.Load<Texture2D>("nuevoAutoJugador");
      jugador_text = cm.Load<Texture2D>("jugador");

      _gameBackgroundTexture = cm.Load<Texture2D>("fondoCiudad");

      jugador = new Jugador(playerTexture,
                            cm.Load<Texture2D>("balaJugador2"),
                            new Vector2(600, 100), 8f,
                            gd.PreferredBackBufferWidth,
                            gd.PreferredBackBufferHeight,
                            100);

      enemigo = new Enemigo(cm.Load<Texture2D>("nuevoAutoPolicia"),
                            cm.Load<Texture2D>("balaPolicia"),
                            new Vector2(400, 700),
                            6f, 100f, 775f);
    }
    public override void Update(GameTime gameTime)
    {
      _backgroundOffsetY += _backgroundSpeed; //moving background

      if (_backgroundOffsetY >= gd.PreferredBackBufferHeight)
      {
          _backgroundOffsetY = 0f;
      }

      jugador.Update(gameTime);
      if (enemigo != null)
      {
          enemigo.Update(gameTime, jugador.Position, jugador);

        if (enemigo.DisparoRealizado)
        {
            //_enemyShootSound.Play();
        }

        if (enemigo.Vida <= 0)
        {
            enemigo = null; 
            victory?.Invoke();
        }
      }

      for (int i = jugador.Balas.Count - 1; i >= 0; i--)
      {
          jugador.Balas[i].Update(gameTime);

          if (enemigo != null && ColisionaConEnemigo(jugador.Balas[i], enemigo))
          {
              //_collisionSound.Play(); // Sonido de colisión al golpear al enemigo
              enemigo.RecibirDaño(1);
              jugador.Balas.RemoveAt(i);

              if (enemigo.Vida <= 0)
              {
                enemigo = null;
                victory?.Invoke();
              }
          }
      }
      _timeSinceLastSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (_timeSinceLastSpawn >= _spawnCooldown)
      {
          int minX = 300;
          int maxX = gd.PreferredBackBufferWidth - 300;

          Random random = new Random();
          float randomX = random.Next(minX, maxX);
          float speed = 1;
          Projectile nuevoObstaculo = new(
              _obstaculoTexture,
              ProjectileType.Obstacle,
              new Vector2(randomX, -_obstaculoTexture.Height),
              new(0,speed),
              speed
          );

          _obstaculos.Add(nuevoObstaculo);
          _timeSinceLastSpawn = 0f;
      }
      for (int i = _obstaculos.Count - 1; i >= 0; i--)
      {
          _obstaculos[i].Update(gameTime);

          if (ColisionaConJugador(_obstaculos[i], jugador))
          {
              //_collisionSound.Play();
              jugador.ReducirVida(1);
              _obstaculos.RemoveAt(i);
          }
          else if (_obstaculos[i].position.Y > gd.PreferredBackBufferHeight)
          {
              _obstaculos.RemoveAt(i);
          }
      }
      if (jugador.Vida <= 0)
      {
          MediaPlayer.Pause(); // Pausa la música al terminar el juego
          gameOver?.Invoke();
      }
      lastKey = Keyboard.GetState();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(
        _gameBackgroundTexture,
        new Rectangle(0, (int)-_backgroundOffsetY,
                      gd.PreferredBackBufferWidth,
                      gd.PreferredBackBufferHeight),
        Color.White
      );

      spriteBatch.Draw(
        _gameBackgroundTexture,
        new Rectangle(0,
                    (int)(-_backgroundOffsetY + gd.PreferredBackBufferHeight),
                    gd.PreferredBackBufferWidth,
                    gd.PreferredBackBufferHeight),
        Color.White
      );

      foreach (var obstaculo in _obstaculos)
      {
        obstaculo.Draw(spriteBatch);
      }

      jugador.Draw(spriteBatch);
      Debugger.Instance.DrawRectHollow(spriteBatch, new((int)jugador.Position.X, (int)jugador.Position.Y, jugador.Texture.Width, jugador.Texture.Height), 4, Color.White);
      jugador.DrawHealthBar(spriteBatch, jugador_text);
      if(enemigo != null)
      {
          enemigo.Draw(spriteBatch);
          enemigo.DrawHealthBar(spriteBatch, jugador_text);
      }
    }

    public override void DrawUI(GameTime gameTime, SpriteBatch spriteBatch)
    { }


    public override void UnloadContent()
    { }
    private static bool ColisionaConJugador(Projectile obstaculo, Jugador jugador)
    {
        float escalaObstaculo = 0.25f;
        float escalaJugador = 0.25f;
        float escalaJugadorAltura = 0.5f;

        Rectangle obstaculoRect = new Rectangle(
            (int)obstaculo.position.X,
            (int)obstaculo.position.Y,
            (int)(obstaculo.texture.Width * escalaObstaculo),
            (int)(obstaculo.texture.Height * escalaObstaculo)
        );

        Rectangle jugadorRect = new Rectangle(
            (int)jugador.Position.X,
            (int)jugador.Position.Y,
            (int)(jugador.Texture.Width * escalaJugador),
            (int)(jugador.Texture.Height * escalaJugadorAltura)
        );
        return obstaculoRect.Intersects(jugadorRect); // Verifica la colisión
    }
    private static bool ColisionaConEnemigo(Projectile bala, Enemigo enemigo)
    {

      Rectangle balaRect = new Rectangle(
          (int)bala.position.X,
          (int)bala.position.Y,
          bala.texture.Width,
          bala.texture.Height
      );

      Rectangle enemigoRect = new Rectangle(
          (int)enemigo.Position.X,
          (int)enemigo.Position.Y,
          enemigo.Texture.Width,
          enemigo.Texture.Height
      );

      return balaRect.Intersects(enemigoRect);
    }
  }
}
