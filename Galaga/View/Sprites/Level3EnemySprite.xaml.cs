﻿namespace Galaga.View.Sprites
{
    /// <summary>
    ///     The Level 3 enemy sprite.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.BaseSprite" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class Level3EnemySprite
    {
        #region Data members

        private const int InitialY = 400;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Level3EnemySprite" /> class.
        /// </summary>
        public Level3EnemySprite() : base(["BaseSprite", "AlternateSprite"])
        {
            this.InitializeComponent();
            Y = InitialY;
        }

        #endregion
    }
}