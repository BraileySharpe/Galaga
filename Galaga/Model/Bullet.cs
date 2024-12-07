using Galaga.View.Sprites;

namespace Galaga.Model;

/// <summary>
///     Represents a bullet in the game.
/// </summary>
public class Bullet : GameObject
{
    #region Data members

    private readonly int speedXDirection = 0;
    private readonly int speedYDirection = 15;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bullet" /> class.
    /// </summary>
    /// <param name="sprite">
    ///     The sprite of the bullet.
    /// </param>
    /// <param name="type">
    ///     The type of the character.
    /// </param>
    public Bullet(BaseSprite sprite, GlobalEnums.CharacterType type)
    {
        Sprite = sprite;

        if (type == GlobalEnums.CharacterType.Player)
        {
            this.speedYDirection = -this.speedYDirection;
        }

        SetSpeed(this.speedXDirection, this.speedYDirection);
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Move the bullet.
    /// </summary>
    public void Move()
    {
        X += SpeedX;
        Y += SpeedY;
    }

    #endregion
}