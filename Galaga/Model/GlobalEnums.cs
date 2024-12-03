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
            Player,

            /// <summary>
            ///     Represents the enemy.
            /// </summary>
            Enemy
        }

        /// <summary>
        ///     Represents the game round.
        /// </summary>
        public enum GameRound
        {
            /// <summary>
            ///     Round 1.
            /// </summary>
            Round1,

            /// <summary>
            ///     Round 2.
            /// </summary>
            Round2,

            /// <summary>
            ///     Round 3.
            /// </summary>
            Round3
        }

        /// <summary>
        ///     Represents the type of ship
        /// </summary>
        public enum ShipType
        {
            /// <summary>
            ///     Represents the player ship.
            /// </summary>
            Player,

            /// <summary>
            ///     Represents the level 1 enemy ship.
            /// </summary>
            Lvl1Enemy = 1,

            /// <summary>
            ///     Represents the level 2 enemy ship.
            /// </summary>
            Lvl2Enemy = 2,

            /// <summary>
            ///     Represents the level 3 enemy ship.
            /// </summary>
            Lvl3Enemy = 3,

            /// <summary>
            ///     Represents the level 4 enemy ship.
            /// </summary>
            Lvl4Enemy = 4,

            /// <summary>
            ///     Represent the bonus enemy ship.
            /// </summary>
            BonusEnemy
        }

        /// <summary>
        ///     Represents the audio file.
        /// </summary>
        public enum AudioFiles
        {
            /// <summary>
            ///     Sound for enemy death.
            /// </summary>
            ENEMY_DEATH,
            /// <summary>
            ///     Sound for enemy shooting.
            /// </summary>
            ENEMY_SHOOT,
            /// <summary>
            ///     Sound for player death.
            /// </summary>
            PLAYER_DEATH,
            /// <summary>
            ///     Sound for player shooting.
            /// </summary>
            PLAYER_SHOOT,
            /// <summary>
            ///    Sound for game over when player loses.
            /// </summary>
            GAMEOVER_LOSE,
            /// <summary>
            ///     Sound for game over when player wins.
            /// </summary>
            GAMEOVER_WIN,
            /// <summary>
            ///     Sound for bonus enemy.
            /// </summary>
            BONUSENEMY_SOUND,
            /// <summary>
            ///     Sound for power up activation.
            /// </summary>
            POWERUP_ACTIVATE,
            /// <summary>
            ///    Sound for power up deactivation.
            /// </summary>
            POWERUP_DEACTIVATE,
            /// <summary>
            ///    Sound for shield hit.
            /// </summary>
            SHIELDHIT
        }

        #endregion
    }
}