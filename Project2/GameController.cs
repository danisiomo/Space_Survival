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

        public GameController(GameModel model)
        {
            _model = model;
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            UpdateShip(keyboardState);
            UpdateAsteroids(gameTime);
            UpdateFuelCans(gameTime);
            UpdateResources();
            CheckCollisions();
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
            // Проверяем столкновения только если игра ещё не окончена
            if (_model.IsGameOver) return;

            foreach (var asteroid in _model.Asteroids)
            {
                if (asteroid.Bounds.Intersects(_model.ShipBounds))
                {
                    _model.IsGameOver = true;
                    return;
                }
            }
        }
    }
}