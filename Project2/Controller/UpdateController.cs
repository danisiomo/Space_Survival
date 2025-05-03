using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using space_survival.Model;

namespace space_survival.Controller
{
    public class UpdateController
    {
        private readonly GameModel _model;
        private float _timeSinceLastAsteroid;
        private float _timeSinceLastFuelSpawn;
        private float _pirateSpawnTimer;
        private bool _wasSpacePressed;
        private float PirateSpawnInterval = 5f; // Каждые 5 секунд
        private float AsteroidSpawnTime = 2f;
        private const float FuelSpawnTime = 10f;
        public UpdateController(GameModel model)
        {
            _model = model;
        }

        public void UpdatePirates(GameTime gameTime)
        {
            _pirateSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_pirateSpawnTimer >= PirateSpawnInterval ||
                _model.Pirates.Count == 0 && _pirateSpawnTimer >= 2f) // Первый пират через 2 сек
            {
                _model.Pirates.Add(new Pirate
                {
                    Position = new Vector2(_model.ScreenWidth, Random.Shared.Next(100, _model.ScreenHeight - 100)),
                    Speed = Random.Shared.Next(3, 6),
                    Texture = _model.PirateTextures[Random.Shared.Next(_model.PirateTextures.Count)] // Случайный спрайт
                });
                _pirateSpawnTimer = 0;

                // Рандомный интервал (3-7 секунд)
                PirateSpawnInterval = Random.Shared.Next(3, 8);
            }

            foreach (var pirate in _model.Pirates.ToList())
            {
                pirate.Update(_model.ShipPosition, (float)gameTime.ElapsedGameTime.TotalSeconds);
                // Обновляем позицию через свойство
                pirate.Position = new Vector2(
                    pirate.Position.X - pirate.Speed,
                    pirate.Position.Y
                );

                pirate.ShootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (pirate.ShootCooldown <= 0)
                {
                    pirate.Bullets.Add(new PirateBullet(
                        startPos: pirate.Position,
                        direction: pirate.Direction // Пули летят в сторону игрока
                    ));
                    pirate.ShootCooldown = 2f;
                }

                if (pirate.Position.X < -100)
                {
                    _model.Pirates.Remove(pirate);
                }
            }
        }
        public void UpdatePirateBullets(GameTime gameTime)
        {
            foreach (var pirate in _model.Pirates.ToList())
            {
                foreach (var bullet in pirate.Bullets.ToList())
                {
                    // Обновляем позицию пули
                    bullet.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (bullet.Position.X < -10 || bullet.Position.X > _model.ScreenWidth)
                        pirate.Bullets.Remove(bullet);
                }
            }
        }
        public void UpdateShip(KeyboardState keyboardState)
        {
            Vector2 movement = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W)) movement.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S)) movement.Y += 1;
            if (keyboardState.IsKeyDown(Keys.A)) movement.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D)) movement.X += 1;

            _model.IsBoosting = keyboardState.IsKeyDown(Keys.LeftShift);
            float speed = _model.IsBoosting ? _model.BoostSpeed : _model.NormalSpeed;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                _model.ShipPosition = new Vector2(
                    _model.ShipPosition.X + movement.X * speed,
                    _model.ShipPosition.Y + movement.Y * speed
                );
                _model.BackgroundOffset = _model.ShipPosition - new Vector2(
                    _model.ScreenWidth / 2f,
                    _model.ScreenHeight / 2f
                );
            }

            // Границы экрана
            _model.ShipPosition = new Vector2(
                MathHelper.Clamp(_model.ShipPosition.X, _model.ShipTexture.Width / 2, _model.ScreenWidth - _model.ShipTexture.Width / 2),
                MathHelper.Clamp(_model.ShipPosition.Y, _model.ShipTexture.Height / 2, _model.ScreenHeight - _model.ShipTexture.Height / 2)
            );
        }
        public void UpdateAsteroids(GameTime gameTime)
        {
            _timeSinceLastAsteroid += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastAsteroid >= AsteroidSpawnTime)
            {
                _model.Asteroids.Add(new Asteroid
                {
                    Position = new Vector2(_model.ScreenWidth, Random.Shared.Next(50, _model.ScreenHeight - 50)),
                    Speed = Random.Shared.Next(1, 4),
                    Texture = _model.AsteroidTextures[Random.Shared.Next(_model.AsteroidTextures.Count)] // Выбор случайной текстуры
                });
                _timeSinceLastAsteroid = 0;
            }

            for (int i = _model.Asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = _model.Asteroids[i];
                asteroid.Position = new Vector2(
                    asteroid.Position.X - asteroid.Speed,
                    asteroid.Position.Y
                );
                asteroid.Update((float)gameTime.ElapsedGameTime.TotalSeconds); // Обновляем вращение

                if (asteroid.Position.X < -50)
                {
                    _model.Asteroids.RemoveAt(i);
                    _model.Score += 10;
                }
            }
        }
        public void UpdateFuelCans(GameTime gameTime)
        {
            _timeSinceLastFuelSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceLastFuelSpawn >= FuelSpawnTime)
            {
                _model.FuelCans.Add(new FuelCan
                {
                    Position = new Vector2(
                        Random.Shared.Next(100, _model.ScreenWidth - 100),
                        Random.Shared.Next(100, _model.ScreenHeight - 100)
                    ),
                    IsCollected = false
                });
                _timeSinceLastFuelSpawn = 0;
            }

            foreach (var can in _model.FuelCans.ToList())
            {
                if (!can.IsCollected && can.Bounds.Intersects(_model.ShipBounds))
                {
                    can.IsCollected = true;
                    _model.Fuel = MathHelper.Clamp(_model.Fuel + 30, 0, 100);
                    _model.Score += 20;
                    _model.FuelCans.Remove(can);
                }
            }
        }
        public void UpdateResources()
        {
            // Только расход топлива
            if (_model.IsBoosting)
                _model.Fuel -= 0.1f;
            else
                _model.Fuel -= 0.05f;

            _model.Fuel = MathHelper.Clamp(_model.Fuel, 0, 100);
        }
    }
}
