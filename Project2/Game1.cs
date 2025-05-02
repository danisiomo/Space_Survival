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
            //_model.BackgroundTexture = Content.Load<Texture2D>("background_1");
            //_model.AsteroidTexture = Content.Load<Texture2D>("asteroid");
            _model.FuelCanTexture = Content.Load<Texture2D>("fuelcan");
            _model.Font = Content.Load<SpriteFont>("font");
            _view = new GameView(_model, _spriteBatch, _controller);

            //_model.PirateTexture = Content.Load<Texture2D>("piratik");
            _model.PirateBulletTexture = Content.Load<Texture2D>("pirate_bull");

            _model.HeartTexture = Content.Load<Texture2D>("heart");

            // Загрузка астероидов (4 варианта)
            _model.AsteroidTextures.Add(Content.Load<Texture2D>("asteroid"));
            _model.AsteroidTextures.Add(Content.Load<Texture2D>("asteroid_2"));
            _model.AsteroidTextures.Add(Content.Load<Texture2D>("asteroid_3"));
            _model.AsteroidTextures.Add(Content.Load<Texture2D>("asteroid_new"));

            // Загрузка пиратов (3 варианта)
            _model.PirateTextures.Add(Content.Load<Texture2D>("piratik"));
            _model.PirateTextures.Add(Content.Load<Texture2D>("pirat"));
            //_model.PirateTextures.Add(Content.Load<Texture2D>("pirate_bull"));
            _model.PirateTextures.Add(Content.Load<Texture2D>("piratiks"));
            //_model.PirateTextures.Add(Content.Load<Texture2D>("pirate_128"));

            _model.EasyBackground = Content.Load<Texture2D>("background_1"); // Для 1 уровня
            _model.HardBackground = Content.Load<Texture2D>("background_hard"); // Для 2 уровня

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

            _view = new GameView(_model, _spriteBatch, _controller);
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
            // Обработка рестарта на экране победы/поражения
            if ((_model.IsGameOver || _model.IsVictory) && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                _controller.FullRestart();
                return;
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

            // Обработка перехода между уровнями
            if (_model.IsVictory)
            {
                var keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.M)) // В меню
                {
                    ExitToMenu();
                }
                else if (keyboardState.IsKeyDown(Keys.N)) // Следующий уровень
                {
                    NextLevel();
                }
                return;
            }
            _view.Update(gameTime); ;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _view.Draw(gameTime); // Передаем gameTime в GameView
            base.Draw(gameTime);
        }

        // В Game1.cs
        private void ExitToMenu()
        {
            // Сбрасываем на легкий уровень
            _model.CurrentLevel = GameModel.GameLevel.Easy;

            // Сбрасываем все игровые состояния
            _model.IsGameStarted = false;
            _model.IsVictory = false;
            _model.IsGameOver = false;
            _model.IsPaused = false;

            // Сбрасываем игровые данные
            _model.Score = 0;
            _model.Fuel = 100f;
            _model.Hearts = 3;

            // Очищаем все объекты
            _controller.RestartGame();

            // Применяем настройки для легкого уровня
            _controller.ApplyLevelSettings();
        }

        private void NextLevel()
        {
            _model.IsVictory = false;
            _model.IsPaused = false;
            _model.Score = 0; // Сбрасываем счёт

            // Переключаем уровень
            _model.CurrentLevel = _model.CurrentLevel == GameModel.GameLevel.Easy
                ? GameModel.GameLevel.Hard
                : GameModel.GameLevel.Easy;

            _controller.RestartGame();
            _controller.ApplyLevelSettings(); // Применяем новые настройки
        }
    }
}