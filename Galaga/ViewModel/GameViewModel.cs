using System.Collections.Generic;
using System.ComponentModel;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Galaga.Model;

namespace Galaga.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        #region Data members

        private readonly Canvas canvas;
        private readonly GameManager gameManager;
        private readonly HashSet<VirtualKey> activeKeys;

        #endregion

        #region Properties

        public int Score
        {
            get => this.gameManager.Score;
            set
            {
                if (this.gameManager.Score != value)
                {
                    this.gameManager.Score = value;
                    this.OnPropertyChanged(nameof(this.Score));
                }
            }
        }

        public bool HasWon
        {
            get => this.gameManager.HasWon;
            set
            {
                if (this.gameManager.HasWon != value)
                {
                    this.gameManager.HasWon = value;
                    this.OnPropertyChanged(nameof(this.HasWon));
                }
            }
        }

        public bool HasLost
        {
            get => this.gameManager.HasLost;
            set
            {
                if (this.gameManager.HasLost != value)
                {
                    this.gameManager.HasLost = value;
                    this.OnPropertyChanged(nameof(this.HasLost));
                }
            }
        }

        #endregion

        #region Constructors

        public GameViewModel(Canvas canvas)
        {
            this.canvas = canvas;
            this.gameManager = new GameManager(canvas);
            this.activeKeys = new HashSet<VirtualKey>();

            this.gameManager.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(this.gameManager.Score))
                {
                    this.OnPropertyChanged(nameof(this.Score));
                }

                if (e.PropertyName == nameof(this.gameManager.HasWon))
                {
                    this.OnPropertyChanged(nameof(this.HasWon));
                }

                if (e.PropertyName == nameof(this.gameManager.HasLost))
                {
                    this.OnPropertyChanged(nameof(this.HasLost));
                }
            };
        }

        #endregion

        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
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

        public void UpdateGameState()
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

        public void StopAllTimers()
        {
            this.gameManager.StopAllTimers();
        }

        #endregion
    }
}