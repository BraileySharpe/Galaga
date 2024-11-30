using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents a player life in the game.
    /// </summary>
    /// <seealso cref="Galaga.Model.GameObject" />
    public class PlayerLife : GameObject
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerLife" /> class.
        /// </summary>
        public PlayerLife()
        {
            Sprite = new PlayerLifeIcon();
        }

        #endregion
    }
}