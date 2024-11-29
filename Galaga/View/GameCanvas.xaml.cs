using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Model;
using Galaga.View.Sprites;

namespace Galaga.View
{
    /// <summary>
    ///     The Canvas for the Galaga Game.
    /// </summary>
    public sealed partial class GameCanvas
    {
        #region Data Members

        private readonly GameManager gameManager;
        private readonly TimeManager timeManager;
        private readonly HashSet<VirtualKey> activeKeys;
        private bool spacePressedPreviously;
        private bool canShoot = true;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameCanvas"/> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();
            this.activeKeys = new HashSet<VirtualKey>();

            this.setupWindowPreferences();

            this.gameManager = new GameManager(this.canvas);
            this.timeManager = new TimeManager(this);
            this.timeManager.InitializeTimers();

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

        public void GameLoop()
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
                if (!this.spacePressedPreviously && this.canShoot)
                {
                    this.gameManager.PlacePlayerBullet();
                    this.canShoot = false;
                    this.spacePressedPreviously = true;
                    this.timeManager.StartPlayerBulletCooldown();
                }
            }
            else
            {
                this.spacePressedPreviously = false;
            }

            this.gameManager.CheckGameStatus();
        }

        public void MovePlayerBullet()
        {
            this.gameManager.MovePlayerBullet();
        }

        public void PlaceEnemyBullet()
        {
            this.gameManager.PlaceEnemyBullet();
        }

        public void MoveEnemyBullet()
        {
            this.gameManager.MoveEnemyBullet();
        }

        public void MoveEnemiesLeft()
        {
            this.gameManager.MoveEnemiesLeft();
        }

        public void MoveEnemiesRight()
        {
            this.gameManager.MoveEnemiesRight();
        }

        public void ToggleSpritesForAnimation()
        {
            this.gameManager.ToggleSpritesForAnimation();
        }

        public void EnablePlayerShooting()
        {
            this.canShoot = true;
        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Add(args.VirtualKey);
        }

        private void coreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Remove(args.VirtualKey);
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
            this.timeManager.StopAllTimers();

            endgameTextBlock.Visibility = Visibility.Visible;
        }

        private void disableAllSprites()
        {
            foreach (var sprite in this.canvas.Children)
            {
                if (sprite is BaseSprite baseSprite)
                {
                    baseSprite.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}