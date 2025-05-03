using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace space_survival.Model
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
            // Вычисляем вектор направления от пирата к игроку
            // playerPosition - позиция корабля игрока, Position - позиция пирата
            // Корректировка угла: добавляем MathHelper.Pi (180 градусов), 
            // чтобы текстура не была перевернутой
            Rotation = MathF.Atan2(Direction.Y, Direction.X) + MathHelper.Pi;
            Position += new Vector2(-Speed, 0) * deltaTime;
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
}
