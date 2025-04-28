using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project2
{
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
            //_model.AsteroidTexture = Content.Load<Texture2D>("asteroid");
            _model.FuelCanTexture = Content.Load<Texture2D>("fuelcan");
            _model.Font = Content.Load<SpriteFont>("font");
            _view = new GameView(_model, _spriteBatch);

            //_model.PirateTexture = Content.Load<Texture2D>("piratik");
            _model.PirateBulletTexture = Content.Load<Texture2D>("pirate_bull");

            _model.HeartTexture = Content.Load<Texture2D>("heart");

            // Загрузка астероидов (например, 3 варианта)
            _model.AsteroidTextures.Add(Content.Load<Texture2D>("asteroid"));
            //_model.AsteroidTextures.Add(Content.Load<Texture2D>("asteroid_2"));
            //_model.AsteroidTextures.Add(Content.Load<Texture2D>("asteroid_3"));

            // Загрузка пиратов (3 варианта)
            _model.PirateTextures.Add(Content.Load<Texture2D>("piratik"));
            _model.PirateTextures.Add(Content.Load<Texture2D>("pirat"));
            //_model.PirateTextures.Add(Content.Load<Texture2D>("pirate_bull"));
            _model.PirateTextures.Add(Content.Load<Texture2D>("piratiks"));
            //_model.PirateTextures.Add(Content.Load<Texture2D>("pirate_128"));

            try
            {
                _model.StartScreenTexture = Content.Load<Texture2D>("start_screen");
            }
            catch
            {
                // Если текстуры нет, создаем программно
                _model.StartScreenTexture = new Texture2D(GraphicsDevice, 1, 1);
                _model.StartScreenTexture.SetData(new[] { Color.Black });
            }

            _view = new GameView(_model, _spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_model.IsRestartRequested)
            {
                _model.IsRestartRequested = false;
                _model.IsGameOver = false;
                _controller.RestartGame();
                _model.TotalTime = 0f;
            }

            // Выход по ESC при Game Over
            if (_model.IsGameOver && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (!_model.IsGameOver)
            {
                _controller.Update(gameTime);
            }
            _view.Update(gameTime); ;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _view.Draw(gameTime); // Передаем gameTime в GameView
            base.Draw(gameTime);
        }
    }
}