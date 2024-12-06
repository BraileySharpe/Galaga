using System;
using System.Collections.Generic;

namespace Galaga.Model
{
    /// <summary>
    ///     Contains data for all rounds in the game.
    /// </summary>
    public class RoundData
    {
        #region Data members

        private const int NumberOfBonusEnemiesPerRound = 1;

        private const int NumberOfLevel1EnemiesInRound1 = 3;
        private const int NumberOfLevel2EnemiesInRound1 = 4;
        private const int NumberOfLevel3EnemiesInRound1 = 4;
        private const int NumberOfLevel4EnemiesInRound1 = 5;

        private const int NumberOfLevel1EnemiesInRound2 = 4;
        private const int NumberOfLevel2EnemiesInRound2 = 5;
        private const int NumberOfLevel3EnemiesInRound2 = 5;
        private const int NumberOfLevel4EnemiesInRound2 = 6;

        private const int NumberOfLevel1EnemiesInRound3 = 5;
        private const int NumberOfLevel2EnemiesInRound3 = 6;
        private const int NumberOfLevel3EnemiesInRound3 = 6;
        private const int NumberOfLevel4EnemiesInRound3 = 7;

        /// <summary>
        ///     Number of enemies per round.
        ///     Key: Round of the game.
        ///     Value: Number of enemies in round. [numLevel1Enemies, numLevel2Enemies, numLevel3Enemies, numLevel4Enemies]
        /// </summary>
        private readonly Dictionary<GlobalEnums.GameRound, int[]> numberOfEnemiesPerRound;

        private readonly IList<GlobalEnums.GameRound> rounds;

        #endregion

        #region Properties

        /// <summary>
        ///     Reference to the current round.
        /// </summary>
        public GlobalEnums.GameRound CurrentRound { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor for the RoundData class.
        /// </summary>
        public RoundData()
        {
            this.numberOfEnemiesPerRound = new Dictionary<GlobalEnums.GameRound, int[]>();
            this.rounds = new List<GlobalEnums.GameRound>();
            this.CurrentRound = GlobalEnums.GameRound.Round1;
            this.initializeGameData();
        }

        #endregion

        #region Methods

        private void initializeGameData()
        {
            this.initializeRoundData(GlobalEnums.GameRound.Round1, new[]
                {
                    NumberOfLevel1EnemiesInRound1, NumberOfLevel2EnemiesInRound1, NumberOfLevel3EnemiesInRound1,
                    NumberOfLevel4EnemiesInRound1, NumberOfBonusEnemiesPerRound
                });
            this.initializeRoundData(GlobalEnums.GameRound.Round2, new[]
                {
                    NumberOfLevel1EnemiesInRound2, NumberOfLevel2EnemiesInRound2, NumberOfLevel3EnemiesInRound2,
                    NumberOfLevel4EnemiesInRound2, NumberOfBonusEnemiesPerRound
                });
            this.initializeRoundData(GlobalEnums.GameRound.Round3, new[]
                {
                    NumberOfLevel1EnemiesInRound3, NumberOfLevel2EnemiesInRound3, NumberOfLevel3EnemiesInRound3,
                    NumberOfLevel4EnemiesInRound3, NumberOfBonusEnemiesPerRound
                });
        }

        private void initializeRoundData(GlobalEnums.GameRound gameRound, int[] numOfEnemiesPerRow)
        {
            this.numberOfEnemiesPerRound.Add(gameRound, numOfEnemiesPerRow);
            this.rounds.Add(gameRound);
        }

        /// <summary>
        ///     Returns int[] for the number of enemies for the current round.
        /// </summary>
        /// <returns>
        ///     int[] for the number of enemies for the current round.
        /// </returns>
        public int[] GetNumEnemiesForCurrentRound()
        {
            return this.numberOfEnemiesPerRound[this.CurrentRound];
        }

        /// <summary>
        ///     Continues game to the next round.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     "No more rounds." if MoveToNextRound is called when the current round is Round3.
        ///     "Invalid round." if the current round is not Round1, Round2, or Round3.
        /// </exception>
        public void MoveToNextRound()
        {
            for (var i = 0; i < this.rounds.Count; i++)
            {
                if (this.CurrentRound == this.rounds[i])
                {
                    if (i + 1 < this.rounds.Count)
                    {
                        this.CurrentRound = this.rounds[i + 1];
                        return;
                    }
                }
            }
        }

        #endregion
    }
}