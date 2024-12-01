using System;
using Windows.UI.Xaml;

namespace Galaga.View.Sprites
{
    /// <summary>
    ///     Represents the player's shield sprite.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.BaseSprite" />
    public sealed partial class ShieldSprite : BaseSprite
    {
        private int currentFrame;
        private DispatcherTimer animationTimer;

        /// <summary>
        ///    Initializes a new instance of the <see cref="Shield"/> class.
        /// </summary>
        public ShieldSprite()
        {
            this.InitializeComponent();
            this.currentFrame = 1;

            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            animationTimer.Tick += this.AnimationTimerOnTick;
        }

        private void AnimationTimerOnTick(object sender, object e)
        {
            currentFrame = (currentFrame % 3) + 1;

            switch (currentFrame)
            {
                case 1:
                    VisualStateManager.GoToState(this, "Frame1Image", true);
                    break;
                case 2:
                    VisualStateManager.GoToState(this, "Frame2Image", true);
                    break;
                case 3:
                    VisualStateManager.GoToState(this, "Frame3Image", true);
                    break;
            }

        }

        /// <summary>
        ///     Starts the shield animation.
        /// </summary>
        public void StartAnimation()
        {
            animationTimer.Start();
        }

        /// <summary>
        ///     Stops the shield animation.
        /// </summary> 
        public void StopAnimation()
        {
            animationTimer.Stop();
        }
    }
}


