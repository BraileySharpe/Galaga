namespace Galaga.View.Sprites
{
    /// <summary>
    ///     The Level 2 enemy sprite.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.Level1EnemySprite" />
    public sealed partial class Level2EnemySprite
    {
        #region Data members

        private const int InitialY = 325;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Level2EnemySprite" /> class.
        /// </summary>
        public Level2EnemySprite()
        {
            this.InitializeComponent();
            Y = InitialY;
        }

        #endregion
    }
}