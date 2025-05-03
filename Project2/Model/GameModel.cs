using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace space_survival.Model
{
    public class GameModel
    {   
        public bool IsGameOver { get; set; } // Флаг завершения игры

        // Корабль
        private Vector2 _shipPosition;
        public Vector2 ShipPosition
        {
            get => _shipPosition;
            set => _shipPosition = value;
        }
        public float NormalSpeed { get; } = 5f;
        public float BoostSpeed { get; } = 10f;
        public bool IsBoosting { get; set; }
        public Texture2D ShipTexture { get; set; }
        public Rectangle ShipBounds => new Rectangle(
            (int)ShipPosition.X - ShipTexture.Width / 2,
            (int)ShipPosition.Y - ShipTexture.Height / 2,
            ShipTexture.Width,
            ShipTexture.Height
        );

        // Ресурсы (топливо + счет)
        public float Fuel { get; set; } = 100f;
        public int Score { get; set; } = 0;

        // Объекты
        public List<FuelCan> FuelCans { get; } = new List<FuelCan>();
        public List<Asteroid> Asteroids { get; } = new List<Asteroid>();

        // Графика
        public Texture2D FuelCanTexture { get; set; }
        public Texture2D AsteroidTexture { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        public Vector2 BackgroundOffset { get; set; }
        public SpriteFont Font { get; set; }

        // Экран
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        //пираты
        public List<Pirate> Pirates { get; } = new();
        public Texture2D PirateTexture { get; set; }
        public Texture2D PirateBulletTexture { get; set; }

        //cердца
        public int Hearts { get; set; } = 3; // Начальное количество жизней
        public Texture2D HeartTexture { get; set; } // Текстура сердца
        public float HitInvulnerabilityTime { get; } = 2.0f; // Длительность неуязвимости

        public bool IsHit { get; set; }
        public float HitCooldown { get; set; }

        public bool IsInvulnerable { get; set; } // Флаг неуязвимости
        public float InvulnerabilityTimer { get; set; } // Таймер неуязвимости
        public float InvulnerabilityDuration { get; } = 2.0f; // Длительность эффекта

        public bool IsPaused { get; set; } //пауза

        public bool IsGameStarted { get; set; } = false;
        public Texture2D StartScreenTexture { get; set; }

        public float SurvivalTime { get; set; } // Время выживания в секундах
        public bool IsRestartRequested { get; set; } // Флаг запроса рестарта
                                                     
        public float TotalTime { get; set; } // Время с начала игры

        public List<Texture2D> AsteroidTextures { get; } = new(); // Список текстур астероидов
        public List<Texture2D> PirateTextures { get; } = new();   // Список текстур пиратов

        public class Heart
        {
            public Vector2 Position { get; set; }
            public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 30, 30); // Размер сердца
            public bool IsCollected { get; set; }
        }

        public List<Heart> HeartsPickups { get; } = new(); // Список сердец на карте
        public float HeartSpawnChance { get; } = 0.2f; // 20% шанс спавна
        public float TimeSinceLastHeartSpawn { get; set; }
        public float HeartSpawnInterval { get; } = 15f; // Проверка каждые 15 секунд
        public int HighScore { get; private set; }
        private const string FilePath = "highscore.txt";

        public GameModel()
        {
            HighScore = LoadHighScore();
        }

        // Загрузка рекорда
        private int LoadHighScore()
        {
            try
            {
                return File.Exists(FilePath) ? int.Parse(File.ReadAllText(FilePath)) : 0;
            }
            catch
            {
                return 0;
            }
        }

        // Сохранение рекорда
        public void UpdateHighScore(int currentScore)
        {
            if (currentScore > HighScore)
            {
                HighScore = currentScore;
                try
                {
                    File.WriteAllText(FilePath, HighScore.ToString());
                }
                catch { /* Игнорируем ошибки записи */ }
            }
        }

        public enum GameLevel { Easy, Hard }
        public GameLevel CurrentLevel { get; set; } = GameLevel.Easy;
        public bool IsVictory { get; set; } // Флаг победы
        public int VictoryScore { get; } = 100; // Очков для перехода

        public float PirateShootCooldown { get; set; } = 2f; // Значение по умолчанию
        public float LevelTime { get; set; } // Время прохождения текущего уровня

        public Texture2D EasyBackground { get; set; }
        public Texture2D HardBackground { get; set; }

        public Texture2D CurrentBackground
        {
            get => CurrentLevel == GameLevel.Easy ? EasyBackground : HardBackground;
        }

        public Color PirateBulletColor
        {
            get => CurrentLevel == GameLevel.Easy ? Color.Red : Color.Blue;
        }
    }
}