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
                case GlobalEnums.ShipType.Player:
                    return new Player();
                case GlobalEnums.ShipType.Lvl1Enemy:
                    return new Enemy(new Level1EnemySprite())
                    {
                        Score = Level1EnemyScore
                    };
                case GlobalEnums.ShipType.Lvl2Enemy:
                    return new Enemy(new Level2EnemySprite())
                    {
                        Score = Level2EnemyScore
                    };
                case GlobalEnums.ShipType.Lvl3Enemy:
                    return new ShootingEnemy(new Level3EnemySprite())
                    {
                        Score = Level3EnemyScore
                    };
                case GlobalEnums.ShipType.Lvl4Enemy:
                    return new ShootingEnemy(new Level4EnemySprite())
                    {
                        Score = Level4EnemyScore
                    };
                case GlobalEnums.ShipType.BonusEnemy:
                    return new BonusEnemy()
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