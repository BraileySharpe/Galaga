using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Galaga.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Galaga.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameCanvas
    {
        #region Data members

        public const int InitialTickEnemyMovement = 5;
        public const int TicksBeforeEnemyDirectionChange = 10;

        private readonly GameManager gameManager;
        private DispatcherTimer enemyTimer;
        private DispatcherTimer bulletTimer;

        private int enemyTickCounter;
        private bool enemyMoveRight;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameCanvas" /> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();
            this.createBulletTimer();
            this.createEnemyTimer();

            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;

            this.gameManager = new GameManager(this.canvas);
        }

        private void createBulletTimer()
        {
            this.bulletTimer = new DispatcherTimer();
            this.bulletTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            this.bulletTimer.Tick += this.bulletTimerTick;
            this.bulletTimer.Start();
        }

        #endregion

        #region Methods

        private void createEnemyTimer()
        {
            this.enemyTimer = new DispatcherTimer();
            this.enemyTimer.Interval = new TimeSpan(0, 0, 0, 0, 350);
            this.enemyTimer.Tick += this.enemyTimerTick;
            this.enemyTimer.Start();
        }

        private void enemyTimerTick(object sender, object e)
        {
            this.enemyTickCounter++;

            if (this.enemyTickCounter <= InitialTickEnemyMovement)
            {
                this.gameManager.MoveEnemiesLeft();
            }
            else if (this.enemyTickCounter < TicksBeforeEnemyDirectionChange)
            {
                this.enemyTickCounter = TicksBeforeEnemyDirectionChange;
            }

            if (this.enemyTickCounter >= TicksBeforeEnemyDirectionChange)
            {
                if (this.enemyTickCounter % TicksBeforeEnemyDirectionChange == 0)
                {
                    this.enemyMoveRight = !this.enemyMoveRight;
                }

                if (this.enemyMoveRight)
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
                case VirtualKey.Space:
                    this.gameManager.PlaceBullet();
                    break;
            }
        }

        private void bulletTimerTick(object sender, object e)
        {
            this.gameManager.MoveBullet();
        }

        #endregion
    }
}