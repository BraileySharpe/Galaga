using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    ///     Handles collision detection in the game.
    /// </summary>
    public class CollisionManager
    {
        private readonly Canvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public CollisionManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        /// <summary>
        ///     Checks if a bullet collides with any enemy in the list.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        /// <param name="enemies">The list of enemies.</param>
        /// <returns>The enemy that collided with the bullet, or null if no collision occurred.</returns>
        public bool CheckEnemyCollision(Bullet bullet, IList<Enemy> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (bullet.CollidesWith(enemy))
                {
                    this.canvas.Children.Remove(enemy.Sprite);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks if an enemy bullet collides with the player.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        /// <param name="player">The player.</param>
        /// <returns>True if the bullet collides with the player, otherwise false.</returns>
        public bool CheckPlayerCollision(Bullet bullet, Player player)
        {
            return bullet.CollidesWith(player);
        }
    }
}