using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project2
{
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

            // Фон
            _spriteBatch.Draw(
                _model.BackgroundTexture,
                new Rectangle(0, 0, _model.ScreenWidth, _model.ScreenHeight),
                Color.White
            );

            // Астероиды
            foreach (var asteroid in _model.Asteroids)
            {
                _spriteBatch.Draw(
                    _model.AsteroidTexture,
                    asteroid.Bounds,
                    Color.White
                );
            }

            // Канистры с топливом
            foreach (var can in _model.FuelCans)
            {
                if (!can.IsCollected)
                {
                    _spriteBatch.Draw(
                        _model.FuelCanTexture,
                        can.Bounds,
                        Color.White
                    );
                }
            }

            // Корабль
            _spriteBatch.Draw(
                _model.ShipTexture,
                _model.ShipPosition,
                null,
                _model.IsBoosting ? Color.Red : Color.White,
                0f,
                new Vector2(_model.ShipTexture.Width / 2, _model.ShipTexture.Height / 2),
                1f,
                SpriteEffects.None,
                0f
            );

            // HUD (без кислорода)
            _spriteBatch.DrawString(
                _model.Font,
                $"Fuel: {(int)_model.Fuel}%  Score: {_model.Score}",
                new Vector2(10, 10),
                Color.White
            );

            if (_model.IsGameOver)
            {
                string gameOverText = "GAME OVER";
                Vector2 textSize = _model.Font.MeasureString(gameOverText);
                _spriteBatch.DrawString(
                    _model.Font,
                    gameOverText,
                    new Vector2(_model.ScreenWidth / 2 - textSize.X / 2,
                               _model.ScreenHeight / 2 - textSize.Y / 2),
                    Color.Red
                );
            }
            _spriteBatch.End();
        }
    }
}