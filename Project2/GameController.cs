using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Project2
{
    public class GameController
    {
        private readonly GameModel _model;
        private float _timeSinceLastAsteroid;
        private const float AsteroidSpawnTime = 2f;
        private float _timeSinceLastFuelSpawn;
        private const float FuelSpawnTime = 10f;
        private bool _wasSpacePressed;

        public GameController(GameModel model)
        {
            _model = model;
        }

        private float _pirateSpawnTimer;
        private float PirateSpawnInterval = 5f; // Каждые 5 секунд

        private void UpdatePirates(GameTime gameTime)
        {
            _pirateSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_pirateSpawnTimer >= PirateSpawnInterval ||
                (_model.Pirates.Count == 0 && _pirateSpawnTimer >= 2f)) // Первый пират через 2 сек
            {
                _model.Pirates.Add(new Pirate
                {
                    Position = new Vector2(
                        _model.ScreenWidth,
                        Random.Shared.Next(100, _model.ScreenHeight - 100)
                    ),
                    Speed = Random.Shared.Next(3, 6) // Разная скорость
                });
                _pirateSpawnTimer = 0;

                // Рандомный интервал (3-7 секунд)
                PirateSpawnInterval = Random.Shared.Next(3, 8);
            }

            foreach (var pirate in _model.Pirates.ToList())
            {
                // Обновляем позицию через свойство
                pirate.Position = new Vector2(
                    pirate.Position.X - pirate.Speed,
                    pirate.Position.Y
                );

                pirate.ShootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (pirate.ShootCooldown <= 0)
                {
                    pirate.Bullets.Add(new PirateBullet
                    {
                        Position = new Vector2(
                            pirate.Position.X - 30,
                            pirate.Position.Y + 5
                        )
                    });
                    pirate.ShootCooldown = 2f;
                }

                if (pirate.Position.X < -100)
                {
                    _model.Pirates.Remove(pirate);
                }
            }
        }

        private void UpdatePirateBullets()
        {
            foreach (var pirate in _model.Pirates.ToList())
            {
                foreach (var bullet in pirate.Bullets.ToList())
                {
                    // Обновляем позицию пули
                    bullet.Position = new Vector2(
                        bullet.Position.X - bullet.Speed,
                        bullet.Position.Y
                    );

                    if (bullet.Position.X < -10)
                    {
                        pirate.Bullets.Remove(bullet);
                    }
                }
            }
        }
        private void CheckBulletCollisions()
        {
            if (_model.IsInvulnerable) return; // Игнорируем урон при неуязвимости

            foreach (var pirate in _model.Pirates.ToList())
            {
                foreach (var bullet in pirate.Bullets.ToList())
                {
                    if (bullet.Bounds.Intersects(_model.ShipBounds))
                    {
                        pirate.Bullets.Remove(bullet);
                        ApplyDamage();
                        return; // Обрабатываем только одно попадание за кадр
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            // Если игра еще не начата
            if (!_model.IsGameStarted)
            {
                if (keyboardState.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
                {
                    _model.IsGameStarted = true;
                }
                return; // Не обновляем игру, пока не начата
            }

            // Обработка паузы по пробелу
            if (keyboardState.IsKeyDown(Keys.Space) && !_wasSpacePressed)
            {
                _model.IsPaused = !_model.IsPaused;
            }
            _wasSpacePressed = keyboardState.IsKeyDown(Keys.Space);

            if (_model.IsPaused) return;

            if (_model.IsInvulnerable)
            {
                _model.InvulnerabilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_model.InvulnerabilityTimer <= 0)
                {
                    _model.IsInvulnerable = false;
                }
            }

            UpdatePirates(gameTime);
            UpdatePirateBullets();
            CheckBulletCollisions();

            UpdateShip(keyboardState);

            UpdateAsteroids(gameTime);
            UpdateFuelCans(gameTime);
            UpdateResources();
            CheckCollisions();

            
            if (_model.IsHit)
            {
                _model.HitCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_model.HitCooldown <= 0)
                {
                    _model.IsHit = false;
                }
            }
        }

        private void UpdateShip(KeyboardState keyboardState)
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

        private void UpdateAsteroids(GameTime gameTime)
        {
            _timeSinceLastAsteroid += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastAsteroid >= AsteroidSpawnTime)
            {
                _model.Asteroids.Add(new Asteroid
                {
                    Position = new Vector2(
                        _model.ScreenWidth,
                        Random.Shared.Next(50, _model.ScreenHeight - 50)
                    ),
                    Speed = Random.Shared.Next(1, 4)
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

                if (asteroid.Position.X < -50)
                {
                    _model.Asteroids.RemoveAt(i);
                    _model.Score += 10;
                }
            }
        }

        private void UpdateFuelCans(GameTime gameTime)
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

        private void UpdateResources()
        {
            // Только расход топлива
            if (_model.IsBoosting)
                _model.Fuel -= 0.1f;
            else
                _model.Fuel -= 0.05f;

            _model.Fuel = MathHelper.Clamp(_model.Fuel, 0, 100);
        }

        private void CheckCollisions()
        {
            if (_model.IsGameOver || _model.IsInvulnerable) return;

            foreach (var asteroid in _model.Asteroids.ToList())
            {
                if (asteroid.Bounds.Intersects(_model.ShipBounds))
                {
                    _model.Asteroids.Remove(asteroid);
                    ApplyDamage();
                    return;
                }
            }

        }
        private void ApplyDamage()
        {
            _model.Hearts--;
            _model.IsInvulnerable = true;
            _model.InvulnerabilityTimer = _model.InvulnerabilityDuration;

            if (_model.Hearts <= 0)
            {
                _model.IsGameOver = true;
            }
        }

    }
}