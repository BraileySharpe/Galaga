using Galaga.Model;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Galaga.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameCanvas
    {
        public const int InitialTickMovement = 5;
        public const int TicksBeforeDirectionChange = 10;

        private readonly GameManager gameManager;
        private DispatcherTimer timer;

        private int tickCounter;
        private bool moveRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCanvas"/> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();
            this.createTimer();

            Width = this.canvas.Width;
            Height= this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;

            this.gameManager = new GameManager(this.canvas);
        }

        private void createTimer()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 350);
            this.timer.Tick += this.timer_Tick;
            this.timer.Start();
        }

        private void timer_Tick(object sender, object e)
        {
            this.tickCounter++;

            if (this.tickCounter <= InitialTickMovement)
            {
                this.gameManager.MoveEnemiesLeft();
            } else if (this.tickCounter < TicksBeforeDirectionChange)
            {
                this.tickCounter = TicksBeforeDirectionChange;
            }

            if (this.tickCounter >= TicksBeforeDirectionChange)
            {
                if (this.tickCounter % TicksBeforeDirectionChange == 0)
                {
                    this.moveRight = !this.moveRight;
                }

                if (this.moveRight)
                {
                    this.gameManager.MoveEnemiesRight();
                }
                else
                {
                    this.gameManager.MoveEnemiesLeft();
                }
            }

        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    this.gameManager.MovePlayerLeft();
                    break;
                case VirtualKey.Right:
                    this.gameManager.MovePlayerRight();
                    break;
            }
        }
    }
}
