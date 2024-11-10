using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the player in the game.
    /// </summary>
    public class PlayerManager
    {
        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        public Player Player { get; private set; }
        public int Score { get; set; } = 0;
        public int Lives { get; private set; } = 3;

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

        private void initializePlayer()
        {
            this.Player = new Player();
            this.canvas.Children.Add(this.Player.Sprite);
            this.placePlayerNearBottomOfBackgroundCentered();
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
    }
}
