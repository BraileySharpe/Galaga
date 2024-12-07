using System.Collections.Generic;

namespace Galaga.View.Sprites;

/// <summary>
///     The Explosion Animation.
/// </summary>
/// <seealso cref="Galaga.View.Sprites.AnimatedSprite" />
/// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
/// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
public sealed partial class ExplosionSprite
{

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExplosionSprite" /> class.
    /// </summary>
    public ExplosionSprite() : base(["Frame3", "Frame1", "Frame2"])
    {
        this.InitializeComponent();
    }

    #endregion
}