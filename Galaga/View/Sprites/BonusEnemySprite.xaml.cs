namespace Galaga.View.Sprites
{
    /// <summary>
    ///     The Bonus Enemy Sprite.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.UserControl" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class BonusEnemySprite
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BonusEnemySprite" /> class.
        /// </summary>
        public BonusEnemySprite()
        {
            this.InitializeComponent();
            Y = 550;
        }

        #endregion
    }
}