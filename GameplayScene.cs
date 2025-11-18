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
    Texture2D _gameBackgroundTexture;
    Texture2D solidColor;
    Jugador jugador;
    Enemigo enemigo;

    List<Projectile> _obstaculos = new();
    Texture2D _obstaculoTexture;
    float _spawnCooldown = 2f;
    float _timeSinceLastSpawn = 0f;

    SceneManager sm;

    float _backgroundOffsetY = 0;
    readonly float _backgroundSpeed = 2;

    Action pause;
    Action gameOver;
    Action victory;

    KeyboardState lastKey;
    SoundEffectInstance _collisionSound;

    public GameplayScene( ContentManager cm, SceneManager sm, GraphicsDeviceManager gd) 
    {
      this.content = cm;
      this.sm = sm;
      this.graphics = gd;
      pause = sm.actionByState[GameState.Paused];
      gameOver = sm.actionByState[GameState.GameOver];
      victory = sm.actionByState[GameState.Victory];
    }

    public override void LoadContent()
    {
      MediaPlayer.Resume();
      _gameBackgroundTexture = content.Load<Texture2D>("fondoCiudad");

      _obstaculoTexture = content.Load<Texture2D>("enemigo");
      solidColor = content.Load<Texture2D>("jugador");

      _collisionSound = content.Load<SoundEffect>("colision").CreateInstance();

      jugador = new Jugador(content.Load<Texture2D>("nuevoAutoJugador"),
                            content.Load<Texture2D>("balaJugador2"),
                            position: new Vector2(600, 100), speed: 8f,
                            bounds: new( 255, 0, 1015-255, graphics.PreferredBackBufferHeight),
                            vida: 10);
      jugador.sfx_disparo =  base.content.Load<SoundEffect>("disparoJugador");// esto debe ser parte de jugador

      enemigo = new Enemigo(content.Load<Texture2D>("nuevoAutoPolicia"),
                            content.Load<Texture2D>("balaPolicia"),
                            new Vector2(400, 700),
                            6f, 100f, 775f);
      enemigo.sfx_disparo = content.Load<SoundEffect>("disparoEnemigo").CreateInstance();
      enemigo.sfx_matado = content.Load<SoundEffect>("FinalBlow").CreateInstance();
    }
    //go on Utils.cs
    void playSFX(SoundEffectInstance sfx, bool waitForFinishLastSFX = false)
    {
      Random r = new();
      sfx.Pitch = (float)r.NextDouble();
      if(waitForFinishLastSFX && sfx.State == SoundState.Playing)
      { return; }
      sfx.Play();
    }

    public override void Update(GameTime gameTime)
    {
      _backgroundOffsetY += _backgroundSpeed; //moving background
      Random random = new();
      if(Keyboard.GetState().IsKeyDown(Keys.P) && lastKey.IsKeyUp(Keys.P))
      { pause?.Invoke(); }

      if (_backgroundOffsetY >= graphics.PreferredBackBufferHeight)
      { _backgroundOffsetY = 0f; }

      jugador.Update(gameTime);
      if (enemigo != null)
      {
        enemigo.Update(gameTime, jugador.position, jugador);

        if (enemigo.Vida <= 0)
        {
            enemigo = null; 
            MediaPlayer.Pause();
            victory?.Invoke();
        }
      }

      for (int i = jugador.balas.Count - 1; i >= 0; i--)
      {
        jugador.balas[i].Update(gameTime);

        if (enemigo != null && ColisionaConEnemigo(jugador.balas[i], enemigo))
        {
          _collisionSound.Play(); // Sonido de colisión al golpear al enemigo
          enemigo.RecibirDaño(1);
          jugador.balas.RemoveAt(i);

          if (enemigo.Vida <= 0)
          {
            enemigo = null;
            MediaPlayer.Pause();
            victory?.Invoke();
          }
        }
      }
      _timeSinceLastSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (_timeSinceLastSpawn >= _spawnCooldown)
      {
        int minX = 300;
        int maxX = graphics.PreferredBackBufferWidth - 300;

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
          _collisionSound.Pitch = (float)random.NextDouble();
          _collisionSound.Play();

          jugador.ReducirVida(1);
          _obstaculos.RemoveAt(i);
        }
        else if (_obstaculos[i].position.Y > graphics.PreferredBackBufferHeight)
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
                      graphics.PreferredBackBufferWidth,
                      graphics.PreferredBackBufferHeight),
        Color.White
      );

      spriteBatch.Draw(
        _gameBackgroundTexture,
        new Rectangle(0,
                    (int)(-_backgroundOffsetY + graphics.PreferredBackBufferHeight),
                    graphics.PreferredBackBufferWidth,
                    graphics.PreferredBackBufferHeight),
        Color.White
      );

      foreach (var obstaculo in _obstaculos)
      { obstaculo.Draw(spriteBatch); }

      jugador.Draw(spriteBatch);
      Debugger.Instance.DrawRectHollow(spriteBatch, new((int)jugador.position.X, (int)jugador.position.Y, jugador.texture.Width, jugador.texture.Height), 4, Color.White);
      jugador.DrawHealthBar(spriteBatch, solidColor);
      if(enemigo != null)
      {
          enemigo.Draw(spriteBatch);
          enemigo.DrawHealthBar(spriteBatch, solidColor);
      }
    }

    public override void DrawUI(GameTime gameTime, SpriteBatch spriteBatch)
    { }
    public override void UnloadContent()
    { }

    private static bool ColisionaConJugador(Projectile obstaculo, Jugador jugador)
    {
        //float escalaObstaculo = 0.25f;
        //float escalaJugador = 0.25f;
        //float escalaJugadorAltura = 0.5f;

        Rectangle obstaculoRect = new Rectangle(
            (int)obstaculo.position.X,
            (int)obstaculo.position.Y,
            (int)(obstaculo.texture.Width), // * escalaObstaculo),
            (int)(obstaculo.texture.Height) // * escalaObstaculo)
        );

        Rectangle jugadorRect = new Rectangle(
            (int)jugador.position.X,
            (int)jugador.position.Y,
            (int)(jugador.texture.Width),// * escalaJugador),
            (int)(jugador.texture.Height)// * escalaJugadorAltura)
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
