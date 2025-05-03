using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using space_survival.Controller;
using space_survival.Model;

namespace space_survival.View
{
    public class GameView
    {
        private readonly GameModel _model;
        private readonly SpriteBatch _spriteBatch;
        private float _totalTime;
        private readonly GameController _controller;
        private readonly DrawView _drawView;

        public GameView(GameModel model, SpriteBatch spriteBatch, GameController controller)
        {
            _model = model;
            _spriteBatch = spriteBatch;
            _controller = controller;
            _drawView = new DrawView(model, spriteBatch);
        }
        public void Update(GameTime gameTime)
        {
            if (!_model.IsPaused) // Считаем время только вне паузы
            {
                _model.TotalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            // Обработка рестарта по клавише R
            if (_model.IsGameOver && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                _model.IsRestartRequested = true;
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (_model.IsGameOver)
            {
                _drawView.DrawGameOverScreen();
            }
            else if (_model.IsVictory)
            {
                _drawView.DrawVictoryScreen();
            }
            else if (!_model.IsGameStarted)
            {
                _drawView.DrawStartScreen();
            }
            else
            {
                // Фон
                _spriteBatch.Draw(
                    _model.CurrentBackground,
                    new Rectangle(0, 0, _model.ScreenWidth, _model.ScreenHeight),
                    Color.White
                );

                // Астероиды
                foreach (var asteroid in _model.Asteroids)
                {
                    _spriteBatch.Draw(
                        asteroid.Texture,
                        asteroid.Position, // Позиция центра
                        null,
                        Color.White,
                        asteroid.Rotation, // Передаем текущий угол
                        new Vector2(asteroid.Texture.Width / 2, asteroid.Texture.Height / 2),
                        0.4f,
                        SpriteEffects.None,
                        0f
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
                Color shipColor = Color.White;

                if (_model.IsBoosting)
                {
                    shipColor = Color.Red;
                }
                else if (_model.IsInvulnerable)
                {
                    // Мигающий зеленоватый эффект
                    float blinkSpeed = 10f; // Скорость мигания
                    float alpha = 0.7f + 0.3f * (float)Math.Sin(_model.InvulnerabilityTimer * blinkSpeed);
                    shipColor = new Color(100, 255, 100) * alpha; // Зеленоватый цвет
                }

                _spriteBatch.Draw(
                    _model.ShipTexture,
                    _model.ShipPosition,
                    null,
                    shipColor,
                    0f,
                    new Vector2(_model.ShipTexture.Width / 2, _model.ShipTexture.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                // Пираты()
                foreach (var pirate in _model.Pirates)
                {
                    // Спрайт пирата
                    _spriteBatch.Draw(
                        pirate.Texture,
                        pirate.Position,
                        null,
                        Color.White,
                        pirate.Rotation, // Угол поворота
                        new Vector2(pirate.Texture.Width / 2, pirate.Texture.Height / 2), // Центр вращения
                        0.1f,
                        SpriteEffects.None,
                        0f
                    );

                    // Пули пирата
                    foreach (var bullet in pirate.Bullets)
                    {
                        _spriteBatch.Draw(
                            _model.PirateBulletTexture,
                            bullet.Position,
                            null,
                            _model.PirateBulletColor,
                            MathF.Atan2(bullet.Direction.Y, bullet.Direction.X) + MathHelper.Pi, // +180°, // Поворот пули
                            new Vector2(_model.PirateBulletTexture.Width / 2, _model.PirateBulletTexture.Height / 2),
                            0.2f,
                            SpriteEffects.None,
                            0f
                        );
                    }
                }

                // HUD
                _spriteBatch.DrawString(
                    _model.Font,
                    $"Fuel: {(int)_model.Fuel}%  Score: {_model.Score}",
                    new Vector2(10, 10),
                    Color.White
                );

                // Сердечки под счетчиком
                int heartSize = 25; // Размер сердечка
                int heartY = 40; // Позиция Y (под текстом топлива)
                int heartSpacing = 5; // Расстояние между сердечками

                // Надпись "Жизни:"
                _spriteBatch.DrawString(
                    _model.Font,
                    "Lives:",
                    new Vector2(10, heartY),
                    Color.White
                );

                // Сердечки после надписи
                int heartsStartX = 10 + (int)_model.Font.MeasureString("Lives: ").X;

                for (int i = 0; i < _model.Hearts; i++)
                {
                    _spriteBatch.Draw(
                        _model.HeartTexture,
                        new Rectangle(
                            heartsStartX + i * (heartSize + heartSpacing),
                            heartY,
                            heartSize,
                            heartSize
                        ),
                        Color.White
                    );
                }

                //новые сердца
                foreach (var heart in _model.HeartsPickups)
                {
                    if (!heart.IsCollected)
                    {
                        _spriteBatch.Draw(
                            _model.HeartTexture,
                            heart.Bounds,
                            Color.White * 0.8f // Легкая прозрачность
                        );
                    }
                }

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

                if (_model.IsPaused)
                {
                    // 1. Затемнение экрана
                    _spriteBatch.Draw(
                        _model.CurrentBackground, // Можно использовать любую текстуру
                        new Rectangle(0, 0, _model.ScreenWidth, _model.ScreenHeight),
                        Color.Black * 0.5f // Полупрозрачное затемнение
                    );

                    // 2. Текст с эффектом пульсации
                    string pauseText = "PAUSE";
                    Vector2 textSize = _model.Font.MeasureString(pauseText);
                    // Увеличиваем размер в 2 раза 
                    float baseScale = 2.0f;
                    // Добавляем легкую пульсацию (опционально)
                    float pulseScale = 0.1f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5);
                    float totalScale = baseScale + pulseScale;

                    // Позиционируем выше центра (на 25% выше центра)
                    Vector2 position = new Vector2(
                        _model.ScreenWidth / 2 - textSize.X * totalScale / 2,
                        _model.ScreenHeight / 3 - textSize.Y * totalScale / 2  // 1/3 высоты вместо центра
                    );
                    // Мерцание 
                    float blink = 0.7f + 0.3f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5);

                    _spriteBatch.DrawString(
                        _model.Font,
                        pauseText,
                        position,
                        Color.Yellow * blink,
                        0f,
                        Vector2.Zero,
                        totalScale,
                        SpriteEffects.None,
                        0f
                    );

                    // 3. Подсказка для продолжения
                    string hintText = "Press SPACE to continue";
                    Vector2 hintSize = _model.Font.MeasureString(hintText);
                    _spriteBatch.DrawString(
                        _model.Font,
                        hintText,
                        new Vector2(
                            _model.ScreenWidth / 2 - hintSize.X / 2,
                            _model.ScreenHeight / 2 + 50
                        ),
                        Color.White * 0.8f
                    );
                }
            }

            _spriteBatch.End();
        }
        
    }
}