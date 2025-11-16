using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ProyectoJuego
{
  public enum GameState {
    Exit,
    MainMenu,
    Playing,
    Reset,
    Paused,
    GameOver,
    Victory
  }

  public class Game1 : Game
  {
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SceneManager sceneManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content"; //this shit is weird..... what the fuck?
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 1000;
        _graphics.ApplyChanges();
    }
    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // tremendo boilerplate, pero esto es mas experimentar
        sceneManager = new(Content, _graphics);
        sceneManager.actionByState[GameState.Exit] = Exit;
        sceneManager.actionByState[GameState.Playing] = StartGame;
        sceneManager.actionByState[GameState.Reset] = ResetGame;
        sceneManager.actionByState[GameState.Paused] = Pause;
        sceneManager.actionByState[GameState.GameOver] = GameOver;
        sceneManager.actionByState[GameState.Victory] = Victory;

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
        //
        // Carga de texturas

        sceneManager.AddScene(new MainMenu(sceneManager));

        Debugger.Initialize(_graphics.GraphicsDevice);
    }
    KeyboardState lastState = new();

    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
          Keyboard.GetState().IsKeyDown(Keys.Escape))
          {Exit();}
      sceneManager.GetScene().Update(gameTime);
      base.Update(gameTime);
    }
    private void ResetGame()
    {
      LoadContent();
      StartGame();
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        sceneManager.GetScene().Draw(gameTime, _spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
    private void StartGame()
    {
      sceneManager.AddScene(new GameplayScene(Content, sceneManager, _graphics));
    }
    private void Pause()
    {
      sceneManager.AddScene(new PauseScene(Content, _graphics));
    }
    private void GameOver()
    {
      sceneManager.AddScene(new GameOverScene(sceneManager));
    }
    private void Victory()
    {
      sceneManager.AddScene(new VictoryScene(sceneManager));
    }
  }
}
