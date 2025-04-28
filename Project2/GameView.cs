using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project2
{
    public class GameView
    {
        private readonly GameModel _model;
        private readonly SpriteBatch _spriteBatch;
        private float _totalTime;

        public GameView(GameModel model, SpriteBatch spriteBatch)
        {
            _model = model;
            _spriteBatch = spriteBatch;
        }

        // Обновляем в GameView (если нет метода Update)
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
                DrawGameOverScreen();
            }

            else
            {
                if (!_model.IsGameStarted)
                {
                    // Рисуем стартовый экран
                    _spriteBatch.Draw(
                        _model.StartScreenTexture,
                        new Rectangle(0, 0, _model.ScreenWidth, _model.ScreenHeight),
                        Color.White
                    );

                    // Текст "Нажмите Пробел"
                    string pressText = "Press SPACE or CLICK to Start";
                    Vector2 textSize = _model.Font.MeasureString(pressText);
                    float scale = 1.5f; // Увеличиваем размер текста
                    float pulse = 0.5f + (float)Math.Sin(_model.TotalTime * 3) * 0.5f; // Эффект пульсации

                    _spriteBatch.DrawString(
                        _model.Font,
                        pressText,
                        new Vector2(
                            _model.ScreenWidth / 2 - (textSize.X * scale) / 2,
                            _model.ScreenHeight - 150 // Поднимаем выше
                        ),
                        Color.White * pulse,
                        0f,
                        Vector2.Zero,
                        scale,
                        SpriteEffects.None,
                        0f
                    );
                }

                else
                {
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
                        // Корабль пирата
                        _spriteBatch.Draw(
                            pirate.Texture, // Случайный спрайт
                            pirate.Bounds,
                            Color.White
                        );

                        // Пули пирата
                        foreach (var bullet in pirate.Bullets)
                        {
                            _spriteBatch.Draw(
                                _model.PirateBulletTexture,
                                bullet.Bounds,
                                Color.Red // Красные пули
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
                            _model.BackgroundTexture, // Можно использовать любую текстуру
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
                            _model.ScreenWidth / 2 - (textSize.X * totalScale) / 2,
                            _model.ScreenHeight / 3 - (textSize.Y * totalScale) / 2  // 1/3 высоты вместо центра
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
            }
            _spriteBatch.End();
        }
        // В классе GameView
        private void DrawGameOverScreen()
        {
            // Затемнение фона
            _spriteBatch.Draw(
                _model.BackgroundTexture,
                new Rectangle(0, 0, _model.ScreenWidth, _model.ScreenHeight),
                Color.Black * 0.5f
            );

            // Текст "GAME OVER"
            string gameOverText = "GAME OVER";
            Vector2 gameOverSize = _model.Font.MeasureString(gameOverText) * 2.0f;
            float gameOverX = (_model.ScreenWidth - gameOverSize.X) / 2;
            float gameOverY = 150;

            _spriteBatch.DrawString(
                _model.Font,
                gameOverText,
                new Vector2(gameOverX, gameOverY),
                Color.Red,
                0f,
                Vector2.Zero,
                2.0f,
                SpriteEffects.None,
                0f
            );

            // --- Статистика ---
            string timeText = $"Time: {_model.SurvivalTime:F1} sec";
            string scoreText = $"Score: {_model.Score}";

            Vector2 timeSize = _model.Font.MeasureString(timeText);
            Vector2 scoreSize = _model.Font.MeasureString(scoreText);

            float statsX = (_model.ScreenWidth - Math.Max(timeSize.X, scoreSize.X)) / 2;
            float statsY = gameOverY + gameOverSize.Y + 50;

            _spriteBatch.DrawString(
                _model.Font,
                timeText,
                new Vector2(statsX, statsY),
                Color.White
            );

            _spriteBatch.DrawString(
                _model.Font,
                scoreText,
                new Vector2(statsX, statsY + timeSize.Y + 10),
                Color.White
            );

            // --- Кнопки ---
            float pulse = 0.5f + (float)Math.Sin(_model.TotalTime * 3) * 0.5f;

            string restartText = "RESTART (Press R)";
            string quitText = "QUIT (Press ESC)";

            Vector2 restartSize = _model.Font.MeasureString(restartText);
            Vector2 quitSize = _model.Font.MeasureString(quitText);

            float buttonX = (_model.ScreenWidth - Math.Max(restartSize.X, quitSize.X)) / 2;
            float buttonY = statsY + scoreSize.Y + 80;

            // "Restart"
            _spriteBatch.DrawString(
                _model.Font,
                restartText,
                new Vector2(buttonX, buttonY),
                Color.Yellow * pulse
            );

            // "Quit"
            _spriteBatch.DrawString(
                _model.Font,
                quitText,
                new Vector2(buttonX, buttonY + restartSize.Y + 20),
                Color.LightGray * pulse
            );
        }
    }
}