using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model;

/// <summary>
///     Manages the Galaga gameplay.
/// </summary>
public class GameManager : INotifyPropertyChanged
{
    #region Data members

    private readonly Canvas canvas;
    private readonly EnemyManager enemyManager;
    private readonly BulletManager bulletManager;
    private readonly TimeManager timeManager;
    private readonly SfxManager sfxManager;
    private readonly RoundData roundData;

    private PlayerManager playerManager;

    private bool endOfRound;
    private bool canShoot;

    #endregion

    #region Properties

    /// <summary>
    ///     Gets or sets a value indicating whether it is the end of a round.
    /// </summary>
    /// <value>
    ///     true if it's the end of a round, false otherwise.
    /// </value>
    public bool EndOfRound
    {
        get => this.endOfRound;
        set
        {
            if (this.endOfRound != value)
            {
                this.endOfRound = value;
                this.OnPropertyChanged(nameof(this.EndOfRound));
            }
        }
    }

    /// <summary>
    ///     Gets or sets the score.
    /// </summary>
    /// <value>
    ///     The score.
    /// </value>
    public int Score { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance has won.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has won; otherwise, <c>false</c>.
    /// </value>
    public bool HasWon { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance has lost.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has lost; otherwise, <c>false</c>.
    /// </value>
    public bool HasLost { get; set; }

    /// <summary>
    ///     Gets the current round.
    /// </summary>
    public int CurrentRoundNumber => (int)this.roundData.CurrentRound;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameManager" /> class.
    /// </summary>
    /// <param name="canvas">
    ///     The canvas.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///     canvas
    /// </exception>
    public GameManager(Canvas canvas)
    {
        this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        this.roundData = new RoundData();
        this.enemyManager = new EnemyManager(canvas, this.roundData);
        this.bulletManager = new BulletManager(canvas);
        this.sfxManager = new SfxManager();
        this.timeManager = new TimeManager(this);
        this.canShoot = true;
        this.enemyManager.PropertyChanged += this.EnemyManagerOnPropertyChanged;
        this.PropertyChanged += this.GameManagerOnPropertyChanged;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void GameManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(this.EndOfRound) && this.EndOfRound)
        {
            this.ResetBonusEnemyTimers();
        }
    }

    private void EnemyManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(this.enemyManager.HasBonusEnemyStartedMoving) &&
            this.enemyManager.HasBonusEnemyStartedMoving)
        {
            this.sfxManager.Play(GlobalEnums.AudioFiles.BonusEnemySound);
        }
    }

    /// <summary>
    ///     Called when [property changed].
    /// </summary>
    /// <param name="propertyName">
    ///     Name of the property.
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///     Starts the game.
    /// </summary>
    public void StartGame()
    {
        this.playerManager = new PlayerManager(this.canvas);
        this.enemyManager.CreateAndPlaceEnemies();
        this.timeManager.InitializeTimers();
    }

    /// <summary>
    ///     Moves the player left.
    /// </summary>
    public void MovePlayerLeft()
    {
        this.playerManager.MovePlayerLeft();
    }

    /// <summary>
    ///     Moves the player right.
    /// </summary>
    public void MovePlayerRight()
    {
        this.playerManager.MovePlayerRight();
    }

    /// <summary>
    ///     Moves the enemies.
    /// </summary>
    public void MoveEnemies()
    {
        this.enemyManager.MoveEnemies();
    }

    /// <summary>
    ///     Moves the bonus enemy.
    /// </summary>
    public void MoveBonusEnemy()
    {
        this.enemyManager.MoveBonusEnemy();
    }

    /// <summary>
    ///     Places the player bullet.
    /// </summary>
    public void PlacePlayerBullet()
    {
        if (this.canShoot)
        {
            var bullet = this.playerManager.Shoot();
            if (this.bulletManager.PlacePlayerBullet(bullet))
            {
                this.sfxManager.Play(GlobalEnums.AudioFiles.PlayerShoot);
            }

            this.canShoot = false;
            this.timeManager.StartPlayerBulletCooldown();
        }
    }

    /// <summary>
    ///     Moves the player bullet.
    /// </summary>
    public async void MovePlayerBullet()
    {
        var collidingBullet = this.bulletManager.MovePlayerBullet(this.enemyManager.Enemies);
        if (collidingBullet != null)
        {
            var enemy = this.enemyManager.CheckWhichEnemyIsShot(collidingBullet);
            if (enemy != null)
            {
                await this.triggerExplosion(enemy.X, enemy.Y);
                this.sfxManager.Play(GlobalEnums.AudioFiles.EnemyDeath);
                this.Score += enemy.Score;

                if (enemy is BonusEnemy)
                {
                    this.playerManager.GainExtraLife();
                    this.sfxManager.Stop(GlobalEnums.AudioFiles.BonusEnemySound);
                    this.playerManager.ActivateShield();
                    this.sfxManager.Play(GlobalEnums.AudioFiles.PowerUpActivate);
                }
            }
        }
    }

    /// <summary>
    ///     Places the enemy bullet.
    /// </summary>
    public void PlaceEnemyBullet()
    {
        if (this.enemyManager.RemainingShootingEnemies != 0)
        {
            var random = new Random();
            var randomIndex = random.Next(0, this.enemyManager.RemainingShootingEnemies);
            var enemy = this.enemyManager.ShootingEnemies[randomIndex];

            Bullet bullet;

            if (enemy.Sprite is BonusEnemySprite && !this.enemyManager.HasBonusEnemyStartedMoving)
            {
                return;
            }

            if (enemy.Sprite is Level4EnemySprite
                || (enemy.Sprite is BonusEnemySprite && this.enemyManager.HasBonusEnemyStartedMoving))
            {
                bullet = enemy.Shoot(this.playerManager.Player);
            }
            else
            {
                bullet = enemy.Shoot();
            }

            this.sfxManager.Play(GlobalEnums.AudioFiles.EnemyShoot);
            this.bulletManager.PlaceEnemyBullet(bullet);
        }
    }

    /// <summary>
    ///     Moves the enemy bullet.
    /// </summary>
    public async Task MoveEnemyBullet()
    {
        if (this.bulletManager.MoveEnemyBullet(this.playerManager.Player))
        {
            if (this.playerManager.HasPowerUp)
            {
                this.playerManager.HandleHitToShield();
                if (this.playerManager.HasPowerUp)
                {
                    this.sfxManager.Play(GlobalEnums.AudioFiles.ShieldHit);
                }
                else
                {
                    this.sfxManager.Play(GlobalEnums.AudioFiles.PowerUpDeactivate);
                }

                return;
            }

            this.sfxManager.Play(GlobalEnums.AudioFiles.PlayerDeath);
            if (this.playerManager.RemainingLives > 0)
            {
                await this.triggerPlayerDeathAndRespawn();
            }
            else
            {
                this.canvas.Children.Remove(this.playerManager.Player.Sprite);
                this.CheckGameStatus();
            }
        }
    }

    private async Task triggerPlayerDeathAndRespawn()
    {
        this.canShoot = false;

        this.canvas.Children.Remove(this.playerManager.Player.Sprite);
        this.bulletManager.RemoveAllBullets();
        this.timeManager.StopAllTimers();

        await this.triggerExplosion(this.playerManager.Player.X, this.playerManager.Player.Y);
        await Task.Delay(1000);

        this.playerManager.RespawnPlayer();
        this.timeManager.StartAllTimers();

        this.canShoot = true;
    }

    private async Task triggerExplosion(double x, double y)
    {
        var explosion = new Explosion(x, y);
        this.canvas.Children.Add(explosion.Sprite);
        await explosion.Explode();
        this.canvas.Children.Remove(explosion.Sprite);
    }

    /// <summary>
    ///     Toggles the sprites for animation.
    /// </summary>
    public void ToggleSpritesForAnimation()
    {
        this.enemyManager.ToggleSpritesForAnimation();
    }

    /// <summary>
    ///     Checks the game status to see whether the game has been won or lost.
    /// </summary>
    public void CheckGameStatus()
    {
        if (this.playerManager.RemainingLives <= 0 && !this.HasLost)
        {
            this.sfxManager.Play(GlobalEnums.AudioFiles.GameOverLose);
            this.HasLost = true;
        }

        if (this.enemyManager.RemainingEnemies == 0 && !this.HasWon)
        {
            switch (this.roundData.CurrentRound)
            {
                case GlobalEnums.GameRound.Round3:
                    this.sfxManager.Play(GlobalEnums.AudioFiles.GameOverWin);
                    this.HasWon = true;

                    break;
                default:
                    this.roundData.MoveToNextRound();
                    this.EndOfRound = true;
                    this.enemyManager.CreateAndPlaceEnemies();
                    this.EndOfRound = false;
                    break;
            }
        }
    }

    /// <summary>
    ///     Stops all timers.
    /// </summary>
    public void StopAllTimers()
    {
        this.timeManager.StopAllTimers();
    }

    /// <summary>
    ///     Resets the bonus enemy timers.
    /// </summary>
    public void ResetBonusEnemyTimers()
    {
        this.timeManager.ResetBonusEnemyTimers();
    }

    /// <summary>
    ///     Called when the player bullet cooldown is complete.
    /// </summary>
    public void PlayerBulletCooldownComplete()
    {
        this.EnableShooting();
    }

    /// <summary>
    ///     Enables the player to shoot.
    /// </summary>
    public void EnableShooting()
    {
        this.canShoot = true;
    }

    #endregion
}