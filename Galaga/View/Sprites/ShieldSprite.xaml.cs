using System;
using Windows.UI.Xaml;

namespace Galaga.View.Sprites
{
    /// <summary>
    ///     Represents the player's shield sprite.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.BaseSprite" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class ShieldSprite
    {
        #region Data members

        private const int TotalFramesInAnimation = 3;

        private int currentFrame;

        private readonly DispatcherTimer animationTimer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShieldSprite" /> class.
        /// </summary>
        public ShieldSprite()
        {
            this.InitializeComponent();
            this.currentFrame = 1;

            this.animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            this.animationTimer.Tick += this.AnimationTimerOnTick;
        }

        #endregion

        #region Methods

        private void AnimationTimerOnTick(object sender, object e)
        {
            this.currentFrame = this.currentFrame % TotalFramesInAnimation + 1;

            switch (this.currentFrame)
            {
                case 1:
                    VisualStateManager.GoToState(this, "Frame1", false);
                    break;
                case 2:
                    VisualStateManager.GoToState(this, "Frame2", false);
                    break;
                case 3:
                    VisualStateManager.GoToState(this, "Frame3", false);
                    break;
            }
        }

        /// <summary>
        ///     Starts the shield animation.
        /// </summary>
        public void StartAnimation()
        {
            this.animationTimer.Start();
        }

        #endregion
    }
}