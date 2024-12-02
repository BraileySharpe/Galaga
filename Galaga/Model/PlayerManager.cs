using Galaga.View.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the player in the game.
    /// </summary>
    public class PlayerManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private const double ShieldOffsetLeft = 23;
        private const double ShieldOffsetTop = 23;
        private const int StartingLives = 3;
        private const int IconsPerRow = 3;
        private const int MaxShieldHits = 2;

        private readonly Canvas canvas;
        private readonly Grid lifeGrid;

        private readonly IList<PlayerLife> lives;
        private readonly double canvasHeight;
        private readonly double canvasWidth;

        private ShieldSprite shield;
        private int shieldHitsRemaining;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        public Player Player { get; private set; }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        public int Score { get; set; } = 0;

        /// <summary>
        ///     Gets the lives.
        /// </summary>
        /// <value>
        ///     The lives.
        /// </value>
        public int RemainingLives => this.lives.Count;

        /// <summary>
        ///     Holds the power up status of the player.
        /// </summary>
        /// <value>
        ///     True if the player has a power up; otherwise, false.
        /// </value>
        public bool hasPowerUp { get; set; } = false;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerManager" /> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public PlayerManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.lives = new List<PlayerLife>();
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;

            this.lifeGrid = this.canvas.Children.OfType<Grid>().FirstOrDefault(g => g.Name == "lifeGrid")
                            ?? throw new InvalidOperationException("No Grid named 'lifeGrid' found in Canvas.");

            this.initializePlayerInformation();
        }

        #endregion

        #region Methods

        private void initializePlayerInformation()
        {
            this.initializePlayer();
            this.initializePlayerLives();
        }

        private void initializePlayer()
        {
            this.Player = ShipFactory.CreateShip(GlobalEnums.ShipType.Player) as Player;
            if (this.Player != null)
            {
                this.canvas.Children.Add(this.Player.Sprite);
                this.placePlayerNearBottomOfBackgroundCentered();
            }
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            this.Player.X = this.canvasWidth / 2 - this.Player.Width / 2.0;
            this.Player.Y = this.canvasHeight - this.Player.Height - PlayerOffsetFromBottom;
        }

        private void initializePlayerLives()
        {
            this.lifeGrid.Children.Clear();
            this.lifeGrid.RowDefinitions.Clear();
            this.lifeGrid.ColumnDefinitions.Clear();

            for (var i = 0; i < StartingLives; i++)
            {
                var row = i / IconsPerRow;
                var column = i % IconsPerRow;

                if (this.lifeGrid.RowDefinitions.Count <= row)
                {
                    this.lifeGrid.RowDefinitions.Add(new RowDefinition());
                }

                if (this.lifeGrid.ColumnDefinitions.Count < IconsPerRow)
                {
                    this.lifeGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                var life = new PlayerLife();
                this.lives.Add(life);

                life.Sprite.Margin = new Thickness(3);

                Grid.SetRow(life.Sprite, row);
                Grid.SetColumn(life.Sprite, column);

                this.lifeGrid.Children.Add(life.Sprite);
            }
        }

        /// <summary>
        ///     Respawns the player if lives remain.
        /// </summary>
        public void RespawnPlayer()
        {
            if (this.RemainingLives > 0)
            {
                var lastLife = this.lives[this.RemainingLives - 1];
                this.lifeGrid.Children.Remove(lastLife.Sprite);
                this.lives.RemoveAt(this.RemainingLives - 1);
            }

            this.canvas.Children.Add(this.Player.Sprite);
            this.placePlayerNearBottomOfBackgroundCentered();
        }

        /// <summary>
        ///     Moves the player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            if (this.Player.X - this.Player.SpeedX > 0)
            {
                this.Player.MoveLeft();
                this.updateShieldPosition();
            }
        }

        /// <summary>
        ///     Moves the player right.
        /// </summary>
        public void MovePlayerRight()
        {
            if (this.Player.X + this.Player.Width + this.Player.SpeedX < this.canvasWidth)
            {
                this.Player.MoveRight();
                this.updateShieldPosition();
            }
        }

        /// <summary>
        ///     Player Shoots a bullet.
        /// </summary>
        public Bullet Shoot()
        {
            return this.Player.Shoot();
        }

        /// <summary>
        ///     Increases Player's life count by 1.
        /// </summary>
        public void GainExtraLife()
        {
            var newLife = new PlayerLife();

            this.lives.Add(newLife);

            var index = this.RemainingLives - 1;

            var row = index / IconsPerRow;
            var column = index % IconsPerRow;

            if (this.lifeGrid.RowDefinitions.Count <= row)
            {
                this.lifeGrid.RowDefinitions.Add(new RowDefinition());
            }

            if (this.lifeGrid.ColumnDefinitions.Count < IconsPerRow)
            {
                this.lifeGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            Grid.SetRow(newLife.Sprite, row);
            Grid.SetColumn(newLife.Sprite, column);
            this.lifeGrid.Children.Add(newLife.Sprite);
        }

        /// <summary>
        ///     Activates the player's shield.
        /// </summary>
        public void ActivateShield()
        {
            if (this.shield == null)
            {
                this.shield = new ShieldSprite();
            }
            
            Canvas.SetLeft(this.shield, this.Player.X - ShieldOffsetLeft);
            Canvas.SetTop(this.shield, this.Player.Y - ShieldOffsetTop);
            Canvas.SetZIndex(this.shield, Canvas.GetZIndex(this.Player.Sprite) + 1);

            if (!this.canvas.Children.Contains(this.shield))
            {
                this.canvas.Children.Add(this.shield);
            }

            this.shield.StartAnimation();

            this.hasPowerUp = true;
            this.shieldHitsRemaining = MaxShieldHits;
        }

        private void updateShieldPosition()
        {
            if (this.hasPowerUp && this.shield != null)
            {
                Canvas.SetLeft(this.shield, this.Player.X - ShieldOffsetLeft);
                Canvas.SetTop(this.shield, this.Player.Y - ShieldOffsetTop);
            }
        }

        /// <summary>
        ///     Deactivates the player's shield.
        /// </summary>
        public void DeactivateShield()
        {
            if (this.shield != null && this.canvas.Children.Contains(this.shield))
            {
                this.canvas.Children.Remove(this.shield);
                this.shield = null;
            }

            this.shieldHitsRemaining = 0;
            this.hasPowerUp = false;
        }

        /// <summary>
        ///     Handles a hit to the player when shield is active.
        /// </summary>
        public void HandleHitToShield()
        {
            if (this.shieldHitsRemaining == 1)
            {
                this.DeactivateShield();
            }
            else
            {
                this.shieldHitsRemaining--;
            }
        }

        #endregion
    }
}