using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NeonShooter;

public class GameRoot : Game
{
    public static GameRoot Instance { get; private set; }
    public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
    public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
    public static GameTime GameTime { get; private set; }

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public GameRoot()
    {
        Instance = this;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        base.Initialize();

        EntityManager.Add(PlayerShip.Instance);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Art.Load(Content);
        Sound.Load(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        GameTime = gameTime;
        Input.Update();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        EntityManager.Update();
        EnemySpawner.Update();
        PlayerStatus.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // Draw entities. Sort by texture for better batching.
        _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
        EntityManager.Draw(_spriteBatch);
        _spriteBatch.End();

        // Draw user interface
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        _spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
        DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
        DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

        // draw the custom mouse cursor 
        _spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

        if (PlayerStatus.IsGameOver)
        {
            string text = "Game Over\n" +
                "Your Score: " + PlayerStatus.Score + "\n" +
                "High Score: " + PlayerStatus.HighScore;

            Vector2 textSize = Art.Font.MeasureString(text);
            _spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawRightAlignedString(string text, float y)
    {
        var textWidth = Art.Font.MeasureString(text).X;
        _spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
    }
}
