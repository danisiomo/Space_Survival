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

        private readonly GameController _controller;

        public GameView(GameModel model, SpriteBatch spriteBatch, GameController controller)
        {
            _model = model;
            _spriteBatch = spriteBatch;
            _controller = controller;
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
            else if (_model.IsVictory)
            {
                DrawVictoryScreen();
            }
            else if (!_model.IsGameStarted)
            {
                DrawStartScreen();
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
                            Color.Red,
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

            _spriteBatch.End();
        }
        private void DrawStartScreen()
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


            // Рекорд
            string highScoreText = $"HIGH SCORE: {_model.HighScore}";
            Vector2 highScoreSize = _model.Font.MeasureString(highScoreText);

            // Параметры анимации
            float scale1 = 1.5f; // Увеличенный размер (было 1.0)
            float alpha = 0.7f + 0.3f * (float)Math.Sin(_model.TotalTime * 3); // Плавное мигание (0.7-1.0)
            _spriteBatch.DrawString(
                _model.Font,
                highScoreText,
                new Vector2(700, 280), // Ваши координаты
                Color.Red * alpha, // Добавляем прозрачность
                0f,
                Vector2.Zero,
                scale1, // Применяем масштаб
                SpriteEffects.None,
                0f
            );

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
        private void DrawGameOverScreen()
        {
            // Затемнение фона
            _spriteBatch.Draw(
                _model.BackgroundTexture,
                new Rectangle(0, 0, _model.ScreenWidth, _model.ScreenHeight),
                Color.Black * 0.5f
            );

            // --- "GAME OVER" с пульсацией ---
            string gameOverText = "GAME OVER";
            Vector2 gameOverSize = _model.Font.MeasureString(gameOverText) * 2.0f;
            float pulse = 0.5f + (float)Math.Sin(_model.TotalTime * 5) * 0.5f; // Увеличиваем частоту мигания (5 вместо 3)

            _spriteBatch.DrawString(
                _model.Font,
                gameOverText,
                new Vector2(
                    _model.ScreenWidth / 2 - gameOverSize.X / 2,
                    150
                ),
                Color.Red * pulse, // Применяем пульсацию только здесь
                0f,
                Vector2.Zero,
                2.0f,
                SpriteEffects.None,
                0f
            );

            // --- Статистика (без эффектов) ---
            string timeText = $"Time: {_model.LevelTime:F1} sec";
            string scoreText = $"Score: {_model.Score}";
            string highScoreText = $"High Score: {_model.HighScore}";

            Vector2 timeSize = _model.Font.MeasureString(timeText);
            Vector2 scoreSize = _model.Font.MeasureString(scoreText);
            Vector2 highScoreSize = _model.Font.MeasureString(highScoreText);

            float statsX = _model.ScreenWidth / 2 - Math.Max(Math.Max(timeSize.X, scoreSize.X), highScoreSize.X) / 2;
            float statsY = 250;

            _spriteBatch.DrawString(_model.Font, timeText, new Vector2(statsX, statsY), Color.White);
            _spriteBatch.DrawString(_model.Font, scoreText, new Vector2(statsX, statsY + 30), Color.White);
            _spriteBatch.DrawString(_model.Font, highScoreText, new Vector2(statsX, statsY + 60), Color.Gold);

            // --- Кнопки (статичные) ---
            string restartText = "RESTART (Press R)";
            string quitText = "QUIT (Press ESC)";

            Vector2 restartSize = _model.Font.MeasureString(restartText);
            Vector2 quitSize = _model.Font.MeasureString(quitText);

            float buttonX = _model.ScreenWidth / 2 - Math.Max(restartSize.X, quitSize.X) / 2;
            float buttonY = statsY + 120;

            // "RESTART" — жёлтый без мигания
            _spriteBatch.DrawString(
                _model.Font,
                restartText,
                new Vector2(buttonX, buttonY),
                Color.GreenYellow
            );

            // "QUIT" — серый без мигания
            _spriteBatch.DrawString(
                _model.Font,
                quitText,
                new Vector2(buttonX, buttonY + 40),
                Color.LightGray
            );
        }
        private void DrawVictoryScreen()
        {
            // Затемнение фона (более темное)
            _spriteBatch.Draw(
                _model.BackgroundTexture,
                new Rectangle(0, 0, _model.ScreenWidth, _model.ScreenHeight),
                Color.Black * 0.8f
            );

            // Яркий пульсирующий заголовок
            string victoryText = $"LEVEL {(_model.CurrentLevel == GameModel.GameLevel.Easy ? "1" : "2")} COMPLETED!";
            Vector2 victorySize = _model.Font.MeasureString(victoryText);
            float pulse = 0.5f + (float)Math.Sin(_model.LevelTime * 5) * 0.5f;

            _spriteBatch.DrawString(
                _model.Font,
                victoryText,
                new Vector2(450, 150),
                Color.YellowGreen, // Яркое золото
                0f,
                Vector2.Zero,
                2.0f,
                SpriteEffects.None,
                0f
            );

            // Статистика
            string timeText = $"Time: {_model.LevelTime:F1} sec";
            string scoreText = $"Score: {_model.Score}";
            string highScoreText = $"High Score: {_model.HighScore}";

            Vector2 timeSize = _model.Font.MeasureString(timeText);
            float statsX = _model.ScreenWidth / 2 - timeSize.X / 2;

            _spriteBatch.DrawString(_model.Font, timeText, new Vector2(statsX, 250), Color.White);
            _spriteBatch.DrawString(_model.Font, scoreText, new Vector2(statsX, 280), Color.White);
            _spriteBatch.DrawString(_model.Font, highScoreText, new Vector2(statsX, 310), Color.Gold);

            // Яркие контрастные кнопки
            string menuText = "MENU (M)";
            string nextActionText = _model.CurrentLevel == GameModel.GameLevel.Easy
                ? "NEXT LEVEL (N)"
                : "RESTART (R)";

            Vector2 menuSize = _model.Font.MeasureString(menuText);
            float buttonX = _model.ScreenWidth / 2 - menuSize.X / 2;

            _spriteBatch.DrawString(
                _model.Font,
                menuText,
                new Vector2(buttonX, 350),
                Color.Lime
            );

            _spriteBatch.DrawString(
                _model.Font,
                nextActionText,
                new Vector2(buttonX, 400),
                Color.Cyan
            );
        }

        private void DrawButton(string text, float y)
        {
            var size = _model.Font.MeasureString(text);
            _spriteBatch.DrawString(_model.Font, text,
                new Vector2(_model.ScreenWidth / 2 - size.X / 2, y),
                Color.White);
        }

        private void StartGame(GameModel.GameLevel level)
        {
            _model.CurrentLevel = level;
            _model.IsGameStarted = true;
            _controller.ApplyLevelSettings();
        }
    }
}