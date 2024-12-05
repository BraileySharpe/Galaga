using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Command;
using Galaga.Model;

namespace Galaga.ViewModel;

public class GameViewModel : INotifyPropertyChanged
{
    #region Data members

    private const int GameLoopTimerIntervalMilliseconds = 16;

    private readonly HighScoreBoard highScoreBoard;

    private DispatcherTimer gameLoopTimer;
    private readonly Canvas canvas;
    private readonly GameManager gameManager;
    private readonly HashSet<VirtualKey> activeKeys;

    private int score;
    private bool hasWon;
    private bool hasLost;

    #endregion

    #region Properties

    public ObservableCollection<HighScoreEntry> HighScores { get; }

    public ICommand SortHighScoresByNameCommand { get; }

    public ICommand SortHighScoresByScoreCommand { get; }

    public ICommand SortHighScoresByLevelCommand { get; }

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
        this.HighScores = new ObservableCollection<HighScoreEntry>();
        this.highScoreBoard = new HighScoreBoard();
        this.setUpGameLoopTimer();

        this.SortHighScoresByNameCommand = new RelayCommand(_ => this.SortHighScoresByNameScoreLevel());
        this.SortHighScoresByScoreCommand = new RelayCommand(_ => this.SortHighScoresByScoreNameLevel());
        this.SortHighScoresByLevelCommand = new RelayCommand(_ => this.SortHighScoresByLevelScoreName());
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

    /// <summary>
    ///     Ends the game.
    /// </summary>
    public void EndGame()
    {
        this.disableAllSprites();
        this.StopAllTimers();
    }

    /// <summary>
    ///     Ends the game.
    /// </summary>
    public async Task EndGameAsync(string playerName)
    {
        var newEntry = new HighScoreEntry(playerName, this.Score, this.gameManager.CurrentRoundNumber);
        var updatedHighScores = await this.highScoreBoard.AddHighScoreAsync(newEntry);
        this.updateHighScoresCollection(updatedHighScores);
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

    public async Task LoadHighScoresAsync()
    {
        var highScores = await this.highScoreBoard.GetHighScoresAsync();
        this.updateHighScoresCollection(highScores);
    }

    private void updateHighScoresCollection(IEnumerable<HighScoreEntry> highScores)
    {
        this.HighScores.Clear();
        foreach (var score in highScores)
        {
            this.HighScores.Add(score);
        }
    }

    public void SortHighScoresByScoreNameLevel()
    {
        var sortedScores = this.HighScores
            .OrderByDescending(h => h.Score)
            .ThenBy(h => h.PlayerName)
            .ThenBy(h => h.LevelCompleted)
            .ToList();
        this.updateHighScoresCollection(sortedScores);
    }

    public void SortHighScoresByNameScoreLevel()
    {
        var sortedScores = this.HighScores
            .OrderBy(h => h.PlayerName)
            .ThenByDescending(h => h.Score)
            .ThenBy(h => h.LevelCompleted)
            .ToList();
        this.updateHighScoresCollection(sortedScores);
    }

    public void SortHighScoresByLevelScoreName()
    {
        var sortedScores = this.HighScores
            .OrderByDescending(h => h.LevelCompleted)
            .ThenByDescending(h => h.Score)
            .ThenBy(h => h.PlayerName)
            .ToList();
        this.updateHighScoresCollection(sortedScores);
    }

    #endregion
}