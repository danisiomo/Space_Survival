﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public List<PirateBullet> Bullets { get; } = new();
        public Rectangle Bounds => new((int)_position.X, (int)_position.Y, 60, 40);
        public float ShootCooldown { get; set; }
    }

    public class PirateBullet
    {
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
        public float Speed { get; } = 7f;
        public Rectangle Bounds => new((int)_position.X, (int)_position.Y, 20, 20);
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
        public Rectangle Bounds => new Rectangle((int)_position.X, (int)_position.Y, 50, 50);
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

    }
}