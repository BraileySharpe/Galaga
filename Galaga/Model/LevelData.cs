using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;

namespace Galaga.Model
{
    /// <summary>
    ///     Contains data for all levels in the game.
    /// </summary>
    public class LevelData
    {
        /// <summary>
        ///     Reference to the current level.
        /// </summary>
        public GlobalEnums.GameLevel CurrentLevel { get; private set; }

        /// <summary>
        ///    Number of enemies per level.
        ///    Key: Level of the game.
        ///    Value: Number of enemies in level. [numLevel1Enemies, numLevel2Enemies, numLevel3Enemies, numLevel4Enemies]
        /// </summary>
        public static readonly Dictionary<GlobalEnums.GameLevel, int[]> NumberOfEnemiesPerLevel = new Dictionary<GlobalEnums.GameLevel, int[]>
        {
            { GlobalEnums.GameLevel.LEVEL1, new int[] { 3, 4, 4, 5 } },
            { GlobalEnums.GameLevel.LEVEL2, new int[] { 4, 5, 5, 6 } },
            { GlobalEnums.GameLevel.LEVEL3, new int[] { 5, 6, 6, 7 } }
        };

        /// <summary>
        ///     Constructor for the LevelData class.
        /// </summary>
        public LevelData()
        {
            this.CurrentLevel = GlobalEnums.GameLevel.LEVEL1;
        }

        /// <summary>
        ///     Returns int[] for the number of enemies for the current level.
        /// </summary>
        /// <returns>
        ///     int[] for the number of enemies for the current level.
        /// </returns>
        public int[] GetNumEnemiesForCurrentLevel()
        {
            return NumberOfEnemiesPerLevel[this.CurrentLevel];
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
            switch (this.CurrentLevel)
            {
                case GlobalEnums.GameLevel.LEVEL1:
                    this.CurrentLevel = GlobalEnums.GameLevel.LEVEL2;
                    break;
                case GlobalEnums.GameLevel.LEVEL2:
                    this.CurrentLevel = GlobalEnums.GameLevel.LEVEL3;
                    break;
                case GlobalEnums.GameLevel.LEVEL3:
                    throw new InvalidOperationException("No more levels.");
                default:
                    throw new InvalidOperationException("Invalid level.");
            }
        }
    }
}
