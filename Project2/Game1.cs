using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project2
{
    // === Model (Данные) ===
    public class GameModel
    {
        public Vector2 ShipPosition { get; set; }
        public float NormalSpeed { get; } = 5f;
        public float BoostSpeed { get; } = 10f; // Ускорение в 2 раза
        public bool IsBoosting { get; set; }
        public Texture2D ShipTexture { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        public Vector2 BackgroundOffset { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
    }

    // === View (Отрисовка) ===
    public class GameView
    {
        private readonly GameModel _model;
        private readonly SpriteBatch _spriteBatch;

        public GameView(GameModel model, SpriteBatch spriteBatch)
        {
            _model = model;
            _spriteBatch = spriteBatch;
        }

        public void Draw()
        {
            _spriteBatch.Begin();

            // Фон с параллаксом
            _spriteBatch.Draw(
                _model.BackgroundTexture,
                -_model.BackgroundOffset * 0.5f,
                Color.White
            );

            // Корабль (красный при ускорении)
            _spriteBatch.Draw(
                _model.ShipTexture,
                _model.ShipPosition,
                _model.IsBoosting ? Color.Red : Color.White
            );

            _spriteBatch.End();
        }
    }

    // === Controller (Логика) ===
    public class GameController
    {
        private readonly GameModel _model;

        public GameController(GameModel model)
        {
            _model = model;
        }

        public void Update()
        {
            var keyboardState = Keyboard.GetState();
            Vector2 movement = Vector2.Zero;

            // Управление WASD
            if (keyboardState.IsKeyDown(Keys.W)) movement.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S)) movement.Y += 1;
            if (keyboardState.IsKeyDown(Keys.A)) movement.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D)) movement.X += 1;

            // Ускорение на Shift
            _model.IsBoosting = keyboardState.IsKeyDown(Keys.LeftShift);
            float speed = _model.IsBoosting ? _model.BoostSpeed : _model.NormalSpeed;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                _model.ShipPosition += movement * speed;
                _model.BackgroundOffset = _model.ShipPosition - new Vector2(
                    _model.ScreenWidth / 2f,
                    _model.ScreenHeight / 2f
                );
            }
        }
    }

    // === Основной класс ===
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameModel _model;
        private GameView _view;
        private GameController _controller;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Установка размера окна
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _model = new GameModel
            {
                ScreenWidth = _graphics.PreferredBackBufferWidth,
                ScreenHeight = _graphics.PreferredBackBufferHeight,
                ShipPosition = new Vector2(
                    _graphics.PreferredBackBufferWidth / 2f,
                    _graphics.PreferredBackBufferHeight / 2f
                )
            };
            _controller = new GameController(_model);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _model.ShipTexture = Content.Load<Texture2D>("ship");
            _model.BackgroundTexture = Content.Load<Texture2D>("background_1");
            _view = new GameView(_model, _spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _controller.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _view.Draw();
            base.Draw(gameTime);
        }
    }
}