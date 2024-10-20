using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///  Represents enemies in the game
    /// </summary>
    public class Enemy : GameObject
    {
        private BaseSprite sprite;
        private const int SpeedXDirection = 10;
        private const int SpeedYDirection = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy"/> class.
        /// </summary>
        /// <param name="sprite">The enemy sprite.</param>
        public Enemy(BaseSprite sprite)
        {
            Sprite = sprite;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }
    }
}
