using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents enemies in the game.
    /// </summary>
    public class BonusEnemy : ShootingEnemy
    {
        #region Data members

        private const int SpeedXDirection = 20;
        private const int BulletSpeed = 15;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BonusEnemy"/> class.
        /// </summary>
        public BonusEnemy() : base(new BonusEnemySprite())
        {
            Sprite = new BonusEnemySprite();
            SetSpeed(SpeedXDirection, 0);
            EnemyBulletSpeed = BulletSpeed;
        }

        #endregion
    }
}