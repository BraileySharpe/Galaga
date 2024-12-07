using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Commands;
using Galaga.Model;
using VirtualKey = Windows.System.VirtualKey;

namespace Galaga.ViewModel;

/// <summary>
///     The game view model.
/// </summary>
public class GameViewModel : INotifyPropertyChanged
{
    #region Data members

    private const int GameLoopTimerIntervalMilliseconds = 16;

    private readonly HighScoreBoard highScoreBoard;
    private readonly Canvas canvas;
    private readonly GameManager gameManager;
    private readonly HashSet<VirtualKey> activeKeys;
    private readonly Action updateParallaxBackground;

    private DispatcherTimer gameLoopTimer;

    private int score;
    private bool isScoreBoardOpen;
    private bool isInStartScreen;
    private bool hasGameStarted;
    private bool hasWon;
    private bool hasLost;

    #endregion

    #region Properties

    /// <summary>
    ///     Gets the high scores.
    /// </summary>
    public ObservableCollection<HighScoreEntry> HighScores { get; }

    /// <summary>
    ///     Gets the sort high scores by name command.
    /// </summary>
    public ICommand SortHighScoresByNameCommand { get; private set; }

    /// <summary>
    ///     Gets the sort high scores by score command.
    /// </summary>
    public ICommand SortHighScoresByScoreCommand { get; private set; }

    /// <summary>
    ///     Gets the sort high scores by level command.
    /// </summary>
    public ICommand SortHighScoresByLevelCommand { get; private set; }

    /// <summary>
    ///     Gets the command to toggle between the high score board and the start screen.
    /// </summary>
    /// <value>
    ///     The command to toggle between the high score board and the start screen.
    /// </value>
    public ICommand ToggleHighScoreBoardCommand { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether this instance has scored high score.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has scored high score; otherwise, <c>false</c>.
    /// </value>
    public bool HasScoredHighScore { get; private set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is in start screen.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is in start screen; otherwise, <c>false</c>.
    /// </value>
    public bool IsInStartScreen
    {
        get => this.isInStartScreen;
        set
        {
            if (this.isInStartScreen != value)
            {
                this.isInStartScreen = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is score board open.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is score board open; otherwise, <c>false</c>.
    /// </value>
    public bool IsScoreBoardOpen
    {
        get => this.isScoreBoardOpen;
        set
        {
            if (this.isScoreBoardOpen != value)
            {
                this.isScoreBoardOpen = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the score.
    /// </summary>
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

    /// <summary>
    ///     Gets or sets a value indicating whether the player has won.
    /// </summary>
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

    /// <summary>
    ///     Gets or sets a value indicating whether the player has lost.
    /// </summary>
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

    /// <summary>
    ///     Gets or sets a value indicating whether the game has started.
    /// </summary>
    public bool HasGameStarted
    {
        get => this.hasGameStarted;
        set
        {
            if (this.hasGameStarted != value)
            {
                this.hasGameStarted = value;
                this.OnPropertyChanged();
            }
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameViewModel" /> class.
    /// </summary>
    /// <param name="canvas">
    ///     The canvas for the game.
    /// </param>
    /// <param name="updateParallaxBackground">
    ///     Action to update the parallax background.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     canvas or updateParallaxBackground is null.
    /// </exception>
    public GameViewModel(Canvas canvas, Action updateParallaxBackground)
    {
        this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        this.updateParallaxBackground = updateParallaxBackground ??
                                        throw new ArgumentNullException(nameof(updateParallaxBackground));
        this.activeKeys = [];
        this.HighScores = [];
        this.highScoreBoard = new HighScoreBoard();

        this.setUpGameLoopTimer();
        this.IsInStartScreen = true;
        this.relayCommands();

        this.gameManager = new GameManager(canvas);
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void relayCommands()
    {
        this.SortHighScoresByNameCommand = new RelayCommand(_ => this.SortHighScoresByNameScoreLevel());
        this.SortHighScoresByScoreCommand = new RelayCommand(_ => this.SortHighScoresByScoreNameLevel());
        this.SortHighScoresByLevelCommand = new RelayCommand(_ => this.SortHighScoresByLevelScoreName());
        this.ToggleHighScoreBoardCommand = new RelayCommand(_ =>
        {
            this.IsScoreBoardOpen = !this.IsScoreBoardOpen;
            this.IsInStartScreen = !this.IsInStartScreen;
        });
    }

    /// <summary>
    ///     Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">
    ///     The name of the property that changed.
    /// </param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///     Handles the key down event.
    /// </summary>
    /// <param name="key">
    ///     The key that was pressed.
    /// </param>
    public void KeyDown(VirtualKey key)
    {
        this.activeKeys.Add(key);
    }

    /// <summary>
    ///     Handles the key up event.
    /// </summary>
    /// <param name="key">
    ///     The key that was released.
    /// </param>
    public void KeyUp(VirtualKey key)
    {
        this.activeKeys.Remove(key);
    }

    /// <summary>
    ///     Sets up the game loop timer.
    /// </summary>
    private void setUpGameLoopTimer()
    {
        this.gameLoopTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(GameLoopTimerIntervalMilliseconds)
        };
        this.gameLoopTimer.Tick += this.GameLoop;
        this.gameLoopTimer.Start();
    }

    /// <summary>
    ///     The game loop to track the player's key presses and game status.
    /// </summary>
    /// <param name="sender">
    ///     The sender of the event.
    /// </param>
    /// <param name="e">
    ///     The event arguments.
    /// </param>
    public void GameLoop(object sender, object e)
    {
        if (this.activeKeys.Contains(VirtualKey.Enter) && !this.HasGameStarted)
        {
            this.gameManager.StartGame();
            this.HasGameStarted = true;
            this.IsScoreBoardOpen = false;
            this.IsInStartScreen = false;
        }

        if (this.HasGameStarted)
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

            this.updateGameStatus();
            this.updateParallaxBackground?.Invoke();
        }
    }

    private void updateGameStatus()
    {
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
        if (!this.isHighScoreBoardFull())
        {
            this.HasScoredHighScore = true;
        }
        else
        {
            foreach (var score in this.HighScores)
            {
                if (this.Score > score.Score)
                {
                    this.HasScoredHighScore = true;
                    break;
                }
            }
        }

        this.disableAllSprites();
        this.StopAllTimers();
    }

    private bool isHighScoreBoardFull()
    {
        return !(this.HighScores.Count < 10);
    }

    /// <summary>
    ///     Ends the game.
    /// </summary>
    public async Task EndGameAsync(string playerName)
    {
        var newEntry = new HighScoreEntry(playerName, this.Score, this.gameManager.CurrentRoundNumber);
        var updatedHighScores = await this.highScoreBoard.AddHighScoreAsync(newEntry);
        this.updateHighScoresCollection(updatedHighScores);
        this.HasScoredHighScore = true;
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

    /// <summary>
    ///     Stops all timers.
    /// </summary>
    public void StopAllTimers()
    {
        this.gameManager.StopAllTimers();
        this.gameLoopTimer?.Stop();
    }

    /// <summary>
    ///     Loads the high scores asynchronously.
    /// </summary>
    /// <returns>
    ///     The task that loads the high scores.
    /// </returns>
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

    /// <summary>
    ///     Sorts the high scores by score, name, and level.
    /// </summary>
    public void SortHighScoresByScoreNameLevel()
    {
        var sortedScores = this.HighScores
            .OrderByDescending(h => h.Score)
            .ThenBy(h => h.PlayerName)
            .ThenBy(h => h.LevelCompleted)
            .ToList();
        this.updateHighScoresCollection(sortedScores);
    }

    /// <summary>
    ///     Sorts the high scores by name, score, and level.
    /// </summary>
    public void SortHighScoresByNameScoreLevel()
    {
        var sortedScores = this.HighScores
            .OrderBy(h => h.PlayerName)
            .ThenByDescending(h => h.Score)
            .ThenBy(h => h.LevelCompleted)
            .ToList();
        this.updateHighScoresCollection(sortedScores);
    }

    /// <summary>
    ///     Sorts the high scores by level, score, and name.
    /// </summary>
    public void SortHighScoresByLevelScoreName()
    {
        var sortedScores = this.HighScores
            .OrderByDescending(h => h.LevelCompleted)
            .ThenByDescending(h => h.Score)
            .ThenBy(h => h.PlayerName)
            .ToList();
        this.updateHighScoresCollection(sortedScores);
    }

    /// <summary>
    ///     Resets the high score board.
    /// </summary>
    /// <returns>
    ///     The task that resets the high score board.
    /// </returns>
    public async Task ResetHighScoreBoard()
    {
        await this.highScoreBoard.ClearHighScoreBoardAsync();
        this.HighScores.Clear();
    }

    #endregion
}