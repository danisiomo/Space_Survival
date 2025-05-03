using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using space_survival.Model;

namespace space_survival.View
{
    public class DrawView
    {
        private readonly GameModel _model;
        private readonly SpriteBatch _spriteBatch;

        public DrawView(GameModel model, SpriteBatch spriteBatch)
        {
            _model = model;
            _spriteBatch = spriteBatch;
        }

        public void DrawStartScreen()
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
                    _model.ScreenWidth / 2 - textSize.X * scale / 2,
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
        public void DrawGameOverScreen()
        {
            // Затемнение фона
            _spriteBatch.Draw(
                _model.CurrentBackground,
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
        public void DrawVictoryScreen()
        {
            // Затемнение фона (более темное)
            _spriteBatch.Draw(
                _model.CurrentBackground,
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

    }
}
