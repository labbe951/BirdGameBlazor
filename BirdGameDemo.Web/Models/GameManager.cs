

namespace BirdGameDemo.Web.Models
{
    public class GameManager
    {
        private readonly int _gravity = 2;

        public event EventHandler MainLoopCompleted;

        public BirdModel Bird { get; private set; }
        public List<PipeModel> Pipes { get; private set; }
        public bool IsRunning { get; private set; } = false;

        public GameManager()
        {
            Bird = new BirdModel();
            Pipes = new List <PipeModel>();
        }

        public async void MainLoop()
        {
            IsRunning = true;
            while (IsRunning)
            {
                MoveObjects();

                CheckForCollisions();

                ManagePipes();

                MainLoopCompleted?.Invoke(this, EventArgs.Empty);
                await Task.Delay(20);
            }
        }

        public void StartGame()
        {
            if (!IsRunning)
            {
                Bird = new BirdModel();
                Pipes = new List<PipeModel>();
                MainLoop();
            }
           
        }

        public void Jump()
        {
            if (IsRunning)
            {
                Bird.Jump();
            }
        }

        void CheckForCollisions()
        {
            if (Bird.IsOnGround())
                GameOver();

            // 1. Kolla om det finns ett rör i mitten.
            var centeredPipe = Pipes.FirstOrDefault(p => p.IsCentered());

            // 2. Det ska finnas en check som kollar om någonring har kolliderat:
            if (centeredPipe != null)
            {
                bool hasCollidedWithBottom = Bird.DistanceFromGround < centeredPipe.GapBottom - 150;
                // +45 är höjden på fågeln. 
                bool hasCollidedWithTop = Bird.DistanceFromGround + 45 > centeredPipe.GapTop - 150;
                // 2a. Nedre röret  2b. Övre röret.
                if (hasCollidedWithBottom || hasCollidedWithTop)
                    GameOver();
            }

        }

        void ManagePipes()
        {
            // Om det inte finns några rör eller det sista röret är mindre än eller lika med 250 px från vänsterkanten så lägg till ett nytt rör.
            if (!Pipes.Any() || Pipes.Last().DistanceFromLeft <= 250)
                Pipes.Add(new PipeModel());

            if(Pipes.First().IsOffScreen())
               Pipes.Remove(Pipes.First());
        }

        void MoveObjects()
        {
            Bird.Fall(_gravity);
            foreach (var pipe in Pipes)
            {
                pipe.Move();
            }
        }

        public void GameOver()
        {
            IsRunning = false;
        }
    }
}
