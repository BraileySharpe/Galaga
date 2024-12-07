namespace Galaga.View.Sprites
{
    /// <summary>
    ///     The Level 4 enemy sprite.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.BaseSprite" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class Level4EnemySprite
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Level4EnemySprite" /> class.
        /// </summary>
        public Level4EnemySprite() : base(["BaseSprite", "AlternateSprite"])
        {
            this.InitializeComponent();
            Y = 475;
        }

        #endregion

    }
}