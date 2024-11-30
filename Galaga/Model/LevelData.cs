using System;
using System.Collections.Generic;

namespace Galaga.Model
{
    /// <summary>
    ///     Contains data for all levels in the game.
    /// </summary>
    public class LevelData
    {
        #region Data members

        /// <summary>
        ///     Number of enemies per level.
        ///     Key: Level of the game.
        ///     Value: Number of enemies in level. [numLevel1Enemies, numLevel2Enemies, numLevel3Enemies, numLevel4Enemies]
        /// </summary>
        private readonly Dictionary<GlobalEnums.GameLevel, int[]> numberOfEnemiesPerLevel;

        private readonly IList<GlobalEnums.GameLevel> levels;

        #endregion

        #region Properties

        /// <summary>
        ///     Reference to the current level.
        /// </summary>
        public GlobalEnums.GameLevel CurrentLevel { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor for the LevelData class.
        /// </summary>
        public LevelData()
        {
            this.numberOfEnemiesPerLevel = new Dictionary<GlobalEnums.GameLevel, int[]>();
            this.levels = new List<GlobalEnums.GameLevel>();
            this.CurrentLevel = GlobalEnums.GameLevel.LEVEL1;
            this.initializeGameData();
        }

        #endregion

        #region Methods

        private void initializeGameData()
        {
            this.initializeLevelData(GlobalEnums.GameLevel.LEVEL1, new[] { 3, 4, 4, 5 });
            this.initializeLevelData(GlobalEnums.GameLevel.LEVEL2, new[] { 4, 5, 5, 6 });
            this.initializeLevelData(GlobalEnums.GameLevel.LEVEL3, new[] { 5, 6, 6, 7 });
        }

        private void initializeLevelData(GlobalEnums.GameLevel gameLevel, int[] numOfEnemiesPerRow)
        {
            this.numberOfEnemiesPerLevel.Add(gameLevel, numOfEnemiesPerRow);
            this.levels.Add(gameLevel);
        }

        /// <summary>
        ///     Returns int[] for the number of enemies for the current level.
        /// </summary>
        /// <returns>
        ///     int[] for the number of enemies for the current level.
        /// </returns>
        public int[] GetNumEnemiesForCurrentLevel()
        {
            return this.numberOfEnemiesPerLevel[this.CurrentLevel];
        }

        /// <summary>
        ///     Continues game to the next level.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     "No more levels." if MoveToNextLevel is called when the current level is LEVEL3.
        ///     "Invalid level." if the current level is not LEVEL1, LEVEL2, or LEVEL3.
        /// </exception>
        public void MoveToNextLevel()
        {
            for (var i = 0; i < this.levels.Count; i++)
            {
                if (this.CurrentLevel == this.levels[i])
                {
                    if (i + 1 < this.levels.Count)
                    {
                        this.CurrentLevel = this.levels[i + 1];
                        return;
                    }
                }
            }
        }

        #endregion
    }
}