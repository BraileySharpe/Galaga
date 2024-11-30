using System;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    public class ShipFactory
    {
        #region Data members

        private const int Level1EnemyScore = 100;
        private const int Level2EnemyScore = 200;
        private const int Level3EnemyScore = 300;
        private const int Level4EnemyScore = 400;
        private const int BonusEnemyScore = 500;

        #endregion

        #region Methods

        /// <summary>
        ///     Creates ships for the game.
        /// </summary>
        /// <param name="shipType">Type of ship to be created.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static GameObject CreateShip(GlobalEnums.ShipType shipType)
        {
            switch (shipType)
            {
                case GlobalEnums.ShipType.PLAYER:
                    return new Player();
                case GlobalEnums.ShipType.LVL1ENEMY:
                    return new Enemy(new Level1EnemySprite())
                    {
                        Score = Level1EnemyScore
                    };
                case GlobalEnums.ShipType.LVL2ENEMY:
                    return new Enemy(new Level2EnemySprite())
                    {
                        Score = Level2EnemyScore
                    };
                case GlobalEnums.ShipType.LVL3ENEMY:
                    return new ShootingEnemy(new Level3EnemySprite())
                    {
                        Score = Level3EnemyScore
                    };
                case GlobalEnums.ShipType.LVL4ENEMY:
                    return new ShootingEnemy(new Level4EnemySprite())
                    {
                        Score = Level4EnemyScore
                    };
                case GlobalEnums.ShipType.BONUSENEMY:
                    return new Enemy(new BonusEnemySprite())
                    {
                        Score = BonusEnemyScore
                    };
                default:
                    throw new NotSupportedException($"{shipType} is not supported as a ship type.");
            }
        }

        #endregion
    }
}