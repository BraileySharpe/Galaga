using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Galaga.Model;

namespace Galaga.View
{
    /// <summary>
    ///     The Canvas for the Galaga Game.
    /// </summary>
    public sealed partial class GameCanvas
    {
        #region Data members

        private readonly GameManager gameManager;
        private readonly HashSet<VirtualKey> activeKeys;

        #endregion

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

            CompositionTarget.Rendering += this.gameLoop;

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

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Add(args.VirtualKey);
        }

        private void coreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Remove(args.VirtualKey);
        }

        private void gameLoop(object sender, object e)
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
                this.endGame("GAME OVER");
            }

            if (e.PropertyName == nameof(this.gameManager.HasWon) && this.gameManager.HasWon)
            {
                this.endGame("YOU WIN!");
            }
        }

        private void endGame(string endgameText)
        {
            this.disableAllSprites();
            CompositionTarget.Rendering -= this.gameLoop;
            this.gameManager.StopAllTimers();

            this.endGameTextBlock.Text = endgameText;
            this.endGameTextBlock.Visibility = Visibility.Visible;
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
    }
}