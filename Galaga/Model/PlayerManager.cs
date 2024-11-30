using System;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the player in the game.
    /// </summary>
    public class PlayerManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;

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
        public int Lives { get; private set; } = 3;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerManager" /> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public PlayerManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.initializePlayer();
        }

        #endregion

        #region Methods

        private void initializePlayer()
        {
            this.Player = ShipFactory.CreateShip(GlobalEnums.ShipType.PLAYER) as Player;
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

        /// <summary>
        ///     Respawns the player if lives remain.
        /// </summary>
        public void RespawnPlayer()
        {
            if (this.Lives > 0)
            {
                this.Lives--;
                this.canvas.Children.Add(this.Player.Sprite);
                foreach (var sprite in this.canvas.Children)
                {
                    if (sprite is PlayerLifeIcon playerLifeIcon)
                    {
                        this.canvas.Children.Remove(playerLifeIcon);
                        break;
                    }
                }

                this.placePlayerNearBottomOfBackgroundCentered();
            }
        }

        /// <summary>
        ///     Moves the player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            if (this.Player.X - this.Player.SpeedX > 0)
            {
                this.Player.MoveLeft();
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
            this.Lives++;
        }

        #endregion
    }
}