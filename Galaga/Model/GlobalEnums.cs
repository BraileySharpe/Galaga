using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents the global enums for the game.
    /// </summary>
    public class GlobalEnums
    {
        /// <summary>
        ///     Represents the type of the character.
        /// </summary>
        public enum  CharacterType
        {
            /// <summary>
            ///     Represents the player.
            /// </summary>
            PLAYER,
            /// <summary>
            ///     Represents the enemy.
            /// </summary>
            ENEMY
        }

        /// <summary>
        ///    Represents the game level.
        /// </summary>
        public enum GameLevel
        {
            /// <summary>
            ///     Level 1.
            /// </summary>
            LEVEL1,
            /// <summary>
            ///    Level 2.
            /// </summary>
            LEVEL2,
            /// <summary>
            ///    Level 3.
            /// </summary>
            LEVEL3
        }
    }
}
