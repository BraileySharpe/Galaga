using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    public class ShipFactory
    {
        public static GameObject createShip(GlobalEnums.ShipType characterType)
        {
            switch (characterType)
            {
                case GlobalEnums.ShipType.PLAYER:
                    return new Player();
                case GlobalEnums.ShipType.LVL1ENEMY:
                    return new Enemy(new Level1EnemySprite());
                case GlobalEnums.ShipType.LVL2ENEMY:
                    return new Enemy(new Level2EnemySprite());
                case GlobalEnums.ShipType.LVL3ENEMY:
                    return new ShootingEnemy(new Level3EnemySprite());
                case GlobalEnums.ShipType.LVL4ENEMY:
                    return new ShootingEnemy(new Level4EnemySprite());
                default:
                    return null;
            }
        }
    }
}
