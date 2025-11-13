using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProyectoJuego.Content;
using System;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ProyectoJuego
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }


    public class Game1 : Game
    {

        private float _backgroundOffsetY = 0;
        private float _backgroundSpeed = 2;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MainMenu _mainMenu;
        private Jugador jugador;
        private GameState currentState;
        private Texture2D _gameBackgroundTexture;
        Texture2D jugador_text;
        private Enemigo enemigo;
        private SpriteFont font;
        private List<Obstaculo> _obstaculos;
        private Texture2D _obstaculoTexture;
        private float _spawnCooldown;
        private float _timeSinceLastSpawn;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _obstaculoTexture = Content.Load<Texture2D>("enemigo");
            jugador_text = Content.Load<Texture2D>("jugador");
            _obstaculos = new List<Obstaculo>();
            _spawnCooldown = 2f;
            _timeSinceLastSpawn = 0f;
            //falta poner el rango en donde sale los obstaculos osea (que no ocupe todo la pantalla sino que solo una parte)
            //_________________________________________________________________________________________
            // Cargar música de fondo
            //_backgroundMusic = Content.Load<Song>("musicaFondo"); // Archivo .mp3 o .wma

            // Cargar efectos de sonido
            //_playerShootSound = Content.Load<SoundEffect>("disparoJugador"); // Archivo .wav
            //_enemyShootSound = Content.Load<SoundEffect>("disparoEnemigo"); // Archivo .wav
            //_collisionSound = Content.Load<SoundEffect>("colision"); // Archivo .wav

            // Inicia la música de fondo
            //MediaPlayer.IsRepeating = true; // Repetir música en bucle
            //MediaPlayer.Volume = 0.5f;     // Ajustar volumen de la música
            //MediaPlayer.Play(_backgroundMusic);
            //________________________________________________________________________________________
            //eso seria para el sonido pero no llegamos a ponerlo
            // Carga de texturas
            Texture2D enemyTexture = Content.Load<Texture2D>("nuevoAutoPolicia");
            Texture2D fireballTexture = Content.Load<Texture2D>("balaPolicia");
            Texture2D playerTexture = Content.Load<Texture2D>("nuevoAutoJugador");
            Texture2D buttonTexture = Content.Load<Texture2D>("button");
            Texture2D bulletTexture = Content.Load<Texture2D>("balaJugador2");
            Texture2D backgroundTexture = Content.Load<Texture2D>("menu");
            _gameBackgroundTexture = Content.Load<Texture2D>("fondoCiudad");
            font = Content.Load<SpriteFont>("font");

            _mainMenu = new MainMenu(buttonTexture, font, backgroundTexture, StartGame, Exit, Stupid);

            jugador = new Jugador(
                playerTexture,
                bulletTexture,
                new Vector2(600, 100),
                8f,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                100
            );

            enemigo = new Enemigo(
                Content.Load<Texture2D>("nuevoAutoPolicia"),
                Content.Load<Texture2D>("balaPolicia"),
                new Vector2(400, 700),
                6f,
                100f,
                775f
            );
            Debugger.Initialize(_graphics.GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var keyboardState = Keyboard.GetState();

            // Pause
            if (keyboardState.IsKeyDown(Keys.P) && currentState != GameState.Paused)
            {
                currentState = GameState.Paused;
            }
            else if (keyboardState.IsKeyDown(Keys.P) && currentState == GameState.Paused)
            {
                currentState = GameState.Playing;
            }

            switch (currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Update(gameTime);
                    MediaPlayer.Pause();
                    break;

                case GameState.Playing:
                    if (MediaPlayer.State == MediaState.Paused)
                    {
                        MediaPlayer.Resume();
                    }

                    _backgroundOffsetY += _backgroundSpeed; //moving background

                    if (_backgroundOffsetY >= _graphics.PreferredBackBufferHeight)
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
                            currentState = GameState.Victory;
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
                                currentState = GameState.Victory;
                            }
                        }
                    }
                    _timeSinceLastSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_timeSinceLastSpawn >= _spawnCooldown)
                    {
                        int minX = 300;
                        int maxX = _graphics.PreferredBackBufferWidth - 300;

                        Random random = new Random();
                        float randomX = random.Next(minX, maxX);
                        Obstaculo nuevoObstaculo = new Obstaculo(
                            _obstaculoTexture,
                            new Vector2(randomX, -_obstaculoTexture.Height),
                            5f
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
                        else if (_obstaculos[i].Position.Y > _graphics.PreferredBackBufferHeight)
                        {
                            _obstaculos.RemoveAt(i);
                        }
                    }
                    if (jugador.Vida <= 0)
                    {
                        MediaPlayer.Pause(); // Pausa la música al terminar el juego
                        currentState = GameState.GameOver;
                    }

                    break;

                case GameState.Paused:
                    MediaPlayer.Pause();
                    break;

                case GameState.GameOver:
                    MediaPlayer.Pause();
                    var keyboardStateGameOver = Keyboard.GetState();
                    if (keyboardStateGameOver.IsKeyDown(Keys.R))
                    {
                        ResetGame();
                    }
                    else if (keyboardStateGameOver.IsKeyDown(Keys.Escape))
                    {
                        Exit();
                    }
                    break;

                case GameState.Victory:
                    var victoryKeyboardState = Keyboard.GetState();
                    if (victoryKeyboardState.IsKeyDown(Keys.R))
                    {
                        ResetGame();
                    }
                    else if (victoryKeyboardState.IsKeyDown(Keys.Escape))
                    {
                        Exit();
                    }
                    break;
            }

            base.Update(gameTime);
        }
        private void ResetGame()
        {
            currentState = GameState.Playing;
            jugador = new Jugador(
                Content.Load<Texture2D>("nuevoAutoJugador"),
                Content.Load<Texture2D>("balaJugador2"),
                new Vector2(600, 100),
                8f,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                1
            );
            enemigo = new Enemigo(
                Content.Load<Texture2D>("nuevoAutoPolicia"),
                Content.Load<Texture2D>("balaPolicia"),
                new Vector2(400, 700),
                6f,
                100f,
                775f
            );

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch);
                    break;
                case GameState.Playing:
                    _spriteBatch.Draw(
                        _gameBackgroundTexture,
                        new Rectangle(0,
                                    (int)-_backgroundOffsetY,
                                    _graphics.PreferredBackBufferWidth,
                                    _graphics.PreferredBackBufferHeight),
                        Color.White
                    );

                    _spriteBatch.Draw(
                        _gameBackgroundTexture,
                        new Rectangle(0,
                                    (int)(-_backgroundOffsetY + _graphics.PreferredBackBufferHeight),
                                    _graphics.PreferredBackBufferWidth,
                                    _graphics.PreferredBackBufferHeight),
                        Color.White
                    );

                    foreach (var obstaculo in _obstaculos)
                    {
                        obstaculo.Draw(_spriteBatch);
                    }

                    jugador.Draw(_spriteBatch);
                    Debugger.Instance.DrawRectHollow(_spriteBatch, new((int)jugador.Position.X, (int)jugador.Position.Y, jugador.Texture.Width, jugador.Texture.Height), 4, Color.White);
                    jugador.DrawHealthBar(_spriteBatch, jugador_text);
                    if(enemigo != null)
                    {
                        enemigo.Draw(_spriteBatch);
                        enemigo.DrawHealthBar(_spriteBatch, jugador_text);
                    }
                    break;
                case GameState.Paused:
                    _spriteBatch.DrawString(font, "PAUSE",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 50,
                                    _graphics.PreferredBackBufferHeight / 2 - 25),
                        Color.White);
                    _spriteBatch.DrawString(font, "Presiona P para reanudar",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100,
                                    _graphics.PreferredBackBufferHeight / 2 + 25),
                        Color.White);
                    break;
                case GameState.GameOver:
                    _spriteBatch.DrawString(font, "Game Over",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100,
                                    _graphics.PreferredBackBufferHeight / 2 - 50),
                        Color.Red);
                    _spriteBatch.DrawString(font, "Presiona R para reiniciar o Esc para salir",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 200,
                                    _graphics.PreferredBackBufferHeight / 2 + 50),
                        Color.White);
                    break;
                case GameState.Victory:
                    _spriteBatch.DrawString(font, "GANASTE!",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100,
                                    _graphics.PreferredBackBufferHeight / 2 - 50),
                        Color.Green);
                    _spriteBatch.DrawString(font, "Presiona R para reiniciar o Esc para salir",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 200,
                                    _graphics.PreferredBackBufferHeight / 2 + 50),
                        Color.White);
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void StartGame()
        {
            currentState = GameState.Playing;
        }
        private void Stupid()
        {
            Console.WriteLine("you are dumb as fuck");
            Exit();
        }
        private bool ColisionaConJugador(Obstaculo obstaculo, Jugador jugador)
        {
            float escalaObstaculo = 0.25f;
            float escalaJugador = 0.25f;
            float escalaJugadorAltura = 0.5f;

            Rectangle obstaculoRect = new Rectangle(
                (int)obstaculo.Position.X,
                (int)obstaculo.Position.Y,
                (int)(_obstaculoTexture.Width * escalaObstaculo),
                (int)(_obstaculoTexture.Height * escalaObstaculo)
            );

            Rectangle jugadorRect = new Rectangle(
                (int)jugador.Position.X,
                (int)jugador.Position.Y,
                (int)(jugador.Texture.Width * escalaJugador),
                (int)(jugador.Texture.Height * escalaJugadorAltura)
            );
            return obstaculoRect.Intersects(jugadorRect); // Verifica la colisión
        }
        private bool ColisionaConEnemigo(DisparoJugador bala, Enemigo enemigo)
        {
            Rectangle balaRect = new Rectangle(
                (int)bala.Position.X,
                (int)bala.Position.Y,
                bala.Texture.Width,
                bala.Texture.Height
            );

            // Rectángulo de colisión para el enemigo, con los ajustes para hacer la colisión más ajustada
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
