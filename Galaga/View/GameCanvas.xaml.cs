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
        private Random random;

        private DispatcherTimer enemyBulletTimer;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer enemyBulletMovementTimer;
        private DispatcherTimer playerBulletTimer;

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

            this.random = new Random();

            this.createPlayerBulletTimer();
            this.createEnemyMovementTimer();
            this.createEnemyBulletTimer();

            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;

            this.gameManager = new GameManager(this.canvas);
        }

        #endregion

        #region Methods

        private void createEnemyBulletTimer()
        {
            this.enemyBulletTimer = new DispatcherTimer();
            this.enemyBulletMovementTimer = new DispatcherTimer();
            this.enemyBulletTimer.Interval = new TimeSpan(0, 0, 0, 1, 500);
            this.enemyBulletMovementTimer.Interval = new TimeSpan(0, 0, 0, 0, 75);
            this.enemyBulletTimer.Tick += this.bullet_TimerTick;
            this.enemyBulletMovementTimer.Tick += this.bulletMovement_TimerTick;
            this.enemyBulletMovementTimer.Start();
            this.enemyBulletTimer.Start();
        }

        private void bullet_TimerTick(object sender, object e)
        {
            this.gameManager.PlaceEnemyBullet();
        }

        private void bulletMovement_TimerTick(object sender, object e)
        {
            this.gameManager.MoveEnemyBullet();
        }

        private void createPlayerBulletTimer()
        {
            this.playerBulletTimer = new DispatcherTimer();
            this.playerBulletTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            this.playerBulletTimer.Tick += this.playerBullet_TimerTick;
            this.playerBulletTimer.Start();
        }

        private void playerBullet_TimerTick(object sender, object e)
        {
            this.gameManager.MovePlayerBullet();
        }

        private void createEnemyMovementTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer();
            this.enemyMovementTimer.Interval = new TimeSpan(0, 0, 0, 0, 350);
            this.enemyMovementTimer.Tick += this.enemyMovement_TimerTick;
            this.enemyMovementTimer.Start();
        }

        private void enemyMovement_TimerTick(object sender, object e)
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
                    this.gameManager.PlacePlayerBullet();
                    break;
            }
        }

        #endregion
    }
}