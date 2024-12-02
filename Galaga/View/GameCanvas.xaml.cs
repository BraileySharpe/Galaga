using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Model;

namespace Galaga.View
{
    /// <summary>
    ///     The Canvas for the Galaga Game.
    /// </summary>
    public sealed partial class GameCanvas
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameCanvas" /> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();
            this.setupWindowPreferences();

            this.activeKeys = new HashSet<VirtualKey>();
            this.gameManager = new GameManager(this.canvas);
            this.setUpGameLoopTimer();

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

            DataContext = this.gameManager;
            this.gameManager.PropertyChanged += this.OnGameManagerPropertyChanged;
        }

        #endregion

        #region Methods

        private void setupWindowPreferences()
        {
            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));
        }

        /// <summary>
        ///     Moves the player bullet.
        /// </summary>
        public void MovePlayerBullet()
        {
            this.gameManager.MovePlayerBullet();
        }

        /// <summary>
        ///     Places the enemy bullet.
        /// </summary>
        public void PlaceEnemyBullet()
        {
            this.gameManager.PlaceEnemyBullet();
        }

        /// <summary>
        ///     Moves the enemy bullet.
        /// </summary>
        public void MoveEnemyBullet()
        {
            this.gameManager.MoveEnemyBullet();
        }

        /// <summary>
        ///     Moves the enemies.
        /// </summary>
        public void MoveEnemies()
        {
            this.gameManager.MoveEnemies();
        }

        /// <summary>
        ///     Moves the bonus enemy.
        /// </summary>
        public void MoveBonusEnemy()
        {
            this.gameManager.MoveBonusEnemy();
        }

        /// <summary>
        ///     Toggles the sprites for animation.
        /// </summary>
        public void ToggleSpritesForAnimation()
        {
            this.gameManager.ToggleSpritesForAnimation();
        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Add(args.VirtualKey);
        }

        private void coreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Remove(args.VirtualKey);
        }

        private void setUpGameLoopTimer()
        {
            this.gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(GameLoopIntervalMilliseconds)
            };
            this.gameLoopTimer.Tick += this.gameLoopTimer_Tick;
            this.gameLoopTimer.Start();
        }

        private void gameLoopTimer_Tick(object sender, object e)
        {
            if (this.activeKeys.Contains(VirtualKey.Left))
            {
                this.gameManager.MovePlayerLeft();
            }

            if (this.activeKeys.Contains(VirtualKey.Right))
            {
                this.gameManager.MovePlayerRight();
            }

            if (this.activeKeys.Contains(VirtualKey.Space))
            {
                this.gameManager.PlacePlayerBullet();
            }

            this.gameManager.CheckGameStatus();
        }

        private void OnGameManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.gameManager.HasLost) && this.gameManager.HasLost)
            {
                this.endGame(this.gameOverTextBlock);
            }

            if (e.PropertyName == nameof(this.gameManager.HasWon) && this.gameManager.HasWon)
            {
                this.endGame(this.youWinTextBlock);
            }
        }

        private void endGame(TextBlock endgameTextBlock)
        {
            this.disableAllSprites();
            this.gameManager.StopAllTimers();
            this.gameLoopTimer?.Stop();

            endgameTextBlock.Visibility = Visibility.Visible;
        }

        private void disableAllSprites()
        {
            foreach (var uiElement in this.canvas.Children)
            {
                if (!(uiElement is TextBlock))
                {
                    uiElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Data Members

        private readonly GameManager gameManager;
        private readonly HashSet<VirtualKey> activeKeys;
        private const int GameLoopIntervalMilliseconds = 16;
        private DispatcherTimer gameLoopTimer;

        #endregion
    }
}