using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using space_survival.Model;
using System;
using System.Linq;
using static space_survival.Model.GameModel;

namespace space_survival.Controller
{
    public class GameController
    {
        private readonly GameModel _model;
        private float _timeSinceLastAsteroid;
        private float AsteroidSpawnTime = 2f;
        private float _timeSinceLastFuelSpawn;
        private const float FuelSpawnTime = 10f;
        private bool _wasSpacePressed;
        private readonly UpdateController _updateController;

        public GameController(GameModel model)
        {
            _model = model;
            _updateController = new UpdateController(model);
        }

        private float _pirateSpawnTimer;
        private float PirateSpawnInterval = 5f; // Каждые 5 секунд

        
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
            // Проверка победы 
            if (_model.Score >= _model.VictoryScore && !_model.IsVictory)
            {
                _model.IsVictory = true;
                _model.IsPaused = true;
                _model.SurvivalTime = _model.LevelTime; // Сохраняем время прохождения
                //return; // Останавливаем игровую логику
            }

            if (!_model.IsPaused && !_model.IsGameOver && !_model.IsVictory)
            {
                _model.LevelTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            // Завершение игры при паузе + ESC
            if (_model.IsPaused && keyboardState.IsKeyDown(Keys.Escape))
            {
                _model.IsGameOver = true;
                _model.SurvivalTime = _model.TotalTime;
                return;
            }

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

            _updateController.UpdatePirates(gameTime);
            _updateController.UpdatePirateBullets(gameTime);
            CheckBulletCollisions();

            _updateController.UpdateShip(keyboardState);

            _updateController.UpdateAsteroids(gameTime);
            _updateController.UpdateFuelCans(gameTime);
            _updateController.UpdateResources();
            CheckCollisions();
            TrySpawnHeart(gameTime);
            CheckHeartCollisions();

            if (_model.IsHit)
            {
                _model.HitCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_model.HitCooldown <= 0)
                {
                    _model.IsHit = false;
                }
            }
            if (_model.Hearts <= 0 || _model.Fuel <= 0)
            {
                _model.IsGameOver = true;
                _model.SurvivalTime = _model.TotalTime; // Используем TotalTime из модели
                _model.UpdateHighScore(_model.Score);
            }

        }
      
        public void RestartGame()
        {
            // Сброс состояния
            _model.Hearts = 3;
            _model.Fuel = 100f;
            _model.Score = 0;
            _model.TotalTime = 0f;
            _model.IsGameOver = false;
            _model.IsPaused = false;
            _model.BackgroundOffset = Vector2.Zero;
            _model.ShipPosition = new Vector2(_model.ScreenWidth / 2f, _model.ScreenHeight / 2f);

            // Очистка списков
            _model.Asteroids.Clear();
            _model.Pirates.Clear();
            _model.FuelCans.Clear();
            

            // Сброс таймеров
            _timeSinceLastAsteroid = 0;
            _pirateSpawnTimer = 0;
            _timeSinceLastFuelSpawn = 0;
            _model.TimeSinceLastHeartSpawn = 0; // Сброс таймера сердец
             
            // Очищаем все сердца на карте
            _model.HeartsPickups.Clear();

            // Очищаем все объекты
            _model.Asteroids.Clear();
            _model.Pirates.Clear();
            _model.FuelCans.Clear();
            _model.HeartsPickups.Clear();

            // Сбрасываем таймеры
            _timeSinceLastAsteroid = 0;
            _pirateSpawnTimer = 0;
            _timeSinceLastFuelSpawn = 0;
            _model.TimeSinceLastHeartSpawn = 0;

            _model.LevelTime = 0f;
            // Применяем текущие настройки уровня
            ApplyLevelSettings();
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
        private void TrySpawnHeart(GameTime gameTime)
        {
            _model.TimeSinceLastHeartSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_model.TimeSinceLastHeartSpawn >= _model.HeartSpawnInterval)
            {
                // Проверяем шанс и что у игрока меньше 3 жизней
                if (Random.Shared.NextSingle() <= _model.HeartSpawnChance && _model.Hearts < 3)
                {
                    _model.HeartsPickups.Add(new Heart
                    {
                        Position = new Vector2(
                            Random.Shared.Next(50, _model.ScreenWidth - 50),
                            Random.Shared.Next(50, _model.ScreenHeight - 50)
                        )
                    });
                }
                _model.TimeSinceLastHeartSpawn = 0;
            }
        }
        private void CheckHeartCollisions()
        {
            foreach (var heart in _model.HeartsPickups.ToList())
            {
                if (!heart.IsCollected && heart.Bounds.Intersects(_model.ShipBounds))
                {
                    heart.IsCollected = true;
                    _model.Hearts = Math.Min(_model.Hearts + 1, 3); // Не больше 3 жизней
                    _model.HeartsPickups.Remove(heart);
                }
            }
        }

        public void ApplyLevelSettings()
        {
            if (_model.CurrentLevel == GameModel.GameLevel.Easy)
            {
                _model.CurrentAsteroidSpawnTime = _model.EasyAsteroidSpawnTime;
                _model.CurrentPirateSpawnInterval = _model.EasyPirateSpawnInterval;
                _model.PirateShootCooldown = 2f;
            }
            else
            {
                _model.CurrentAsteroidSpawnTime = _model.HardAsteroidSpawnTime;
                _model.CurrentPirateSpawnInterval = _model.HardPirateSpawnInterval;
                _model.PirateShootCooldown = 1f;
            }
        }
        public void FullRestart()
        {
            // Сброс всех игровых объектов
            _model.Asteroids.Clear();
            _model.Pirates.Clear();
            _model.FuelCans.Clear();
            _model.HeartsPickups.Clear();

            // Сброс состояния
            _model.IsGameOver = false;
            _model.IsVictory = false;
            _model.IsPaused = false;
            _model.Score = 0;
            _model.Fuel = 100f;
            _model.Hearts = 3;
            // Сбрасываем таймеры
            _timeSinceLastAsteroid = 0;
            _pirateSpawnTimer = 0;
            _timeSinceLastFuelSpawn = 0;
            _model.TimeSinceLastHeartSpawn = 0;

            _model.LevelTime = 0f;
            // Перезагрузка уровня
            _model.ShipPosition = new Vector2(_model.ScreenWidth / 2f, _model.ScreenHeight / 2f);
            ApplyLevelSettings(); // Важно для применения сложности
        }
    }
}