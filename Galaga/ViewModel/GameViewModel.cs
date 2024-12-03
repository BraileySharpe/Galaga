using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Model;

namespace Galaga.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        #region Data members

        private const int GameLoopTimerIntervalMilliseconds = 16;

        private DispatcherTimer gameLoopTimer;
        private readonly Canvas canvas;
        private readonly GameManager gameManager;
        private readonly HashSet<VirtualKey> activeKeys;

        private int score;
        private bool hasWon;
        private bool hasLost;
        private string endGameText;

        #endregion

        #region Properties

        public string EndGameText
        {
            get => this.endGameText;
            set
            {
                if (this.endGameText != value)
                {
                    this.endGameText = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int Score
        {
            get => this.score;
            set
            {
                if (this.score != value)
                {
                    this.score = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool HasWon
        {
            get => this.hasWon;
            set
            {
                if (this.hasWon != value)
                {
                    this.hasWon = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool HasLost
        {
            get => this.hasLost;
            set
            {
                if (this.hasLost != value)
                {
                    this.hasLost = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public GameViewModel(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.gameManager = new GameManager(canvas);
            this.activeKeys = new HashSet<VirtualKey>();
            this.setUpGameLoopTimer();
        }

        #endregion

        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void KeyDown(VirtualKey key)
        {
            this.activeKeys.Add(key);
        }

        public void KeyUp(VirtualKey key)
        {
            this.activeKeys.Remove(key);
        }

        private void setUpGameLoopTimer()
        {
            this.gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(GameLoopTimerIntervalMilliseconds)
            };
            this.gameLoopTimer.Tick += this.GameLoop;
            this.gameLoopTimer.Start();
        }

        public void GameLoop(object sender, object e)
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

            this.Score = this.gameManager.Score;
            this.HasWon = this.gameManager.HasWon;
            this.HasLost = this.gameManager.HasLost;
            this.gameManager.CheckGameStatus();
        }

        public void EndGame(string endgameText)
        {
            this.disableAllSprites();
            this.StopAllTimers();

            this.EndGameText = endgameText;
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

        public void StopAllTimers()
        {
            this.gameManager.StopAllTimers();
            this.gameLoopTimer?.Stop();
        }

        #endregion
    }
}