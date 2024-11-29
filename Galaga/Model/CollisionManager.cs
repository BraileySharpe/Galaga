using System.Collections.Generic;

namespace Galaga.Model
{
    /// <summary>
    ///     Handles collision detection in the game.
    /// </summary>
    public class CollisionManager
    {
        #region Methods

        /// <summary>
        ///     Checks if an object collides with any enemy in the list.
        /// </summary>
        /// <param name="obj">The obj to check collision</param>
        /// <param name="enemies">The list of enemies.</param>
        /// <returns>True if an enemy is hit, false otherwise</returns>
        public bool CheckEnemyCollision(GameObject obj, IEnumerable<Enemy> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (obj.CollidesWith(enemy))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks if an object collides with the player.
        /// </summary>
        /// <param name="obj">The object to check collision</param>
        /// <param name="player">The player.</param>
        /// <returns>True if the bullet collides with the player, otherwise false.</returns>
        public bool CheckPlayerCollision(GameObject obj, Player player)
        {
            return obj.CollidesWith(player);
        }

        #endregion
    }
}