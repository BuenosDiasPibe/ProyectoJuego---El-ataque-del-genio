using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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

    private KeyboardState prevState;

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
        var _backgroundMusic = Content.Load<Song>("musicaFondo");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.5f;
        MediaPlayer.Play(_backgroundMusic);

        sceneManager.AddScene(new MainMenu(sceneManager));
        Debugger.Initialize(_graphics.GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
          Keyboard.GetState().IsKeyDown(Keys.Escape))
        {Exit();}
      if(Keyboard.GetState().IsKeyDown(Keys.F3) && prevState.IsKeyUp(Keys.F3))
      {
        Debugger.Instance.canDraw = !Debugger.Instance.canDraw; 
      }

      sceneManager.GetScene().Update(gameTime);

      prevState = Keyboard.GetState();
      base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      _spriteBatch.Begin();
      sceneManager.GetScene().Draw(gameTime, _spriteBatch);
      _spriteBatch.End();

      base.Draw(gameTime);
    }
    //wrapper region
    private void ResetGame()
    {
      LoadContent();
      StartGame();
    }
    private void StartGame()
    {
      sceneManager.AddScene(new GameplayScene(Content, sceneManager, _graphics));
    }
    private void Pause()
    {
      sceneManager.AddScene(new PauseScene(sceneManager, Content, _graphics));
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
