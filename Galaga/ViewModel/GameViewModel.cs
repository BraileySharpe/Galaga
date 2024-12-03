using System.Collections.Generic;
using System.ComponentModel;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Galaga.Model;
using System.Runtime.CompilerServices;

namespace Galaga.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        #region Data members

        private readonly Canvas canvas;
        private readonly GameManager gameManager;
        private readonly HashSet<VirtualKey> activeKeys;

        private int score;
        private bool hasWon;
        private bool hasLost;

        #endregion

        #region Properties

        public int Score
        {
            get => this.score;
            set
            {
                if (this.score != value)
                {
                    this.score = value;
                    this.OnPropertyChanged(nameof(this.Score));
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
                    this.OnPropertyChanged(nameof(this.HasWon));
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

            this.Score = this.gameManager.Score;
            this.HasWon = this.gameManager.HasWon;
            this.HasLost = this.gameManager.HasLost;
            this.gameManager.CheckGameStatus();
        }

        public void StopAllTimers()
        {
            this.gameManager.StopAllTimers();
        }

        #endregion
    }
}