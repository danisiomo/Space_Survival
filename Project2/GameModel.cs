using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project2
{
    public class Pirate
    {
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
        public float Speed { get; set; } = 3f;
        public Texture2D Texture { get; set; } 
        public List<PirateBullet> Bullets { get; } = new();
        public Rectangle Bounds => new((int)_position.X, (int)_position.Y, 60, 40);
        public float ShootCooldown { get; set; }
        public float Rotation { get; private set; } // Угол поворота в радианах
        public Vector2 Direction { get; private set; } // Направление движения

        public void Update(Vector2 playerPosition, float deltaTime)
        {
            Direction = Vector2.Normalize(playerPosition - Position);

            // Корректировка угла: добавляем MathHelper.Pi (180 градусов), 
            // чтобы текстура не была перевернутой
            Rotation = MathF.Atan2(Direction.Y, Direction.X) + MathHelper.Pi;

            // Движение (если нужно)
            Position += new Vector2(-Speed, 0) * deltaTime; // Пираты всегда летят влево
        }
    }

    public class PirateBullet
    {
        private Vector2 _position;
        public Vector2 Direction { get; } // Направление движения
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
        public float Speed { get; } = 7f;
        public Rectangle Bounds => new((int)_position.X, (int)_position.Y, 20, 20);

        public PirateBullet(Vector2 startPos, Vector2 direction)
        {
            Position = startPos;
            Direction = direction;
        }

        public void Update(float deltaTime)
        {
            Position += Direction * Speed;
        }
    }

    public class Asteroid
    {
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
        public float Speed { get; set; } = 2f;
        public Texture2D Texture { get; set; } // Новая property
        public float Rotation { get; private set; } // Текущий угол
        public float RotationSpeed { get; } // Скорость вращения (радианы/сек)
        public Rectangle Bounds => new Rectangle((int)_position.X, (int)_position.Y, 50, 50);
        public Asteroid()
        {
            // Случайная скорость вращения (-3 до 3 радиан/сек)
            RotationSpeed = (Random.Shared.NextSingle() - 0.5f) * 6f;
        }

        public void Update(float deltaTime)
        {
            Rotation += RotationSpeed * deltaTime; // Обновляем угол
        }
    }

    public class FuelCan
    {
        public Vector2 Position { get; set; }
        public bool IsCollected { get; set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 40, 60);
    }

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
    }
}