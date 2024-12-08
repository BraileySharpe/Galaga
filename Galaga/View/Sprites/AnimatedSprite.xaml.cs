using System.Collections.Generic;
using Windows.UI.Xaml;

namespace Galaga.View.Sprites;

/// <summary>
///     Sprite to enable animation.
/// </summary>
/// <seealso cref="Galaga.View.Sprites.BaseSprite" />
public partial class AnimatedSprite
{
    #region Data members

    private readonly IList<string> spriteStates;
    private int currentSpriteIndex;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AnimatedSprite" /> class.
    /// </summary>
    /// <param name="spriteStates">
    ///     The sprite states
    /// </param>
    public AnimatedSprite(IList<string> spriteStates)
    {
        this.InitializeComponent();
        this.spriteStates = spriteStates;
        this.currentSpriteIndex = 0;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Toggles the sprite for animation.
    /// </summary>
    public void ToggleSprite()
    {
        this.currentSpriteIndex = (this.currentSpriteIndex + 1) % this.spriteStates.Count;
        VisualStateManager.GoToState(this, this.spriteStates[this.currentSpriteIndex], true);
    }

    #endregion
}