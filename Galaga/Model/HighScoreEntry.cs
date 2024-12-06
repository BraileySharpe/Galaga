using System.Runtime.Serialization;

namespace Galaga.Model;

[DataContract]
public class HighScoreEntry
{
    #region Properties

    /// <summary>
    ///     Gets or sets the name of the player.
    /// </summary>
    /// <value>
    ///     The name of the player.
    /// </value>
    [DataMember(Order = 1)]
    public string PlayerName { get; set; }

    /// <summary>
    ///     Gets or sets the score.
    /// </summary>
    /// <value>
    ///     The score.
    /// </value>
    [DataMember(Order = 2)]
    public int Score { get; set; }

    /// <summary>
    ///     Gets or sets the level completed.
    /// </summary>
    /// <value>
    ///     The level completed.
    /// </value>
    [DataMember(Order = 3)]
    public int LevelCompleted { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="HighScoreEntry"/> class.
    /// </summary>
    /// <param name="playerName">
    ///     The name of the player.
    /// </param>
    /// <param name="score">
    ///     The score.
    /// </param>
    /// <param name="levelCompleted">
    ///     The level completed.
    /// </param>
    public HighScoreEntry(string playerName, int score, int levelCompleted)
    {
        this.PlayerName = playerName;
        this.Score = score;
        this.LevelCompleted = levelCompleted;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HighScoreEntry"/> class.
    /// </summary>
    public HighScoreEntry()
    {
    }

    #endregion
}