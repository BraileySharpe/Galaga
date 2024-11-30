namespace Galaga.Model
{
    /// <summary>
    ///     Represents the global enums for the game.
    /// </summary>
    public class GlobalEnums
    {
        #region Types and Delegates

        /// <summary>
        ///     Represents the type of the character.
        /// </summary>
        public enum CharacterType
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
        ///     Represents the game level.
        /// </summary>
        public enum GameLevel
        {
            /// <summary>
            ///     Level 1.
            /// </summary>
            LEVEL1,

            /// <summary>
            ///     Level 2.
            /// </summary>
            LEVEL2,

            /// <summary>
            ///     Level 3.
            /// </summary>
            LEVEL3
        }

        public enum ShipType
        {
            /// <summary>
            ///     Represents the player ship.
            /// </summary>
            PLAYER,

            /// <summary>
            ///     Represents the level 1 enemy ship.
            /// </summary>
            LVL1ENEMY,

            /// <summary>
            ///     Represents the level 2 enemy ship.
            /// </summary>
            LVL2ENEMY,

            /// <summary>
            ///     Represents the level 3 enemy ship.
            /// </summary>
            LVL3ENEMY,

            /// <summary>
            ///     Represents the level 4 enemy ship.
            /// </summary>
            LVL4ENEMY,

            /// <summary>
            ///     Represent the bonus enemy ship.
            /// </summary>
            BONUSENEMY
        }

        #endregion
    }
}