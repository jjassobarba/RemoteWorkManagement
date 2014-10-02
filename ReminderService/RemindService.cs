using System.ServiceProcess;
using System.Threading;

namespace ReminderService
{
    public class RemindService: ServiceBase
    {
        private Test test;
        private Timer stateTimer;
        private TimerCallback timerDelegate;

        public RemindService()
        {
            ServiceName = "ReminderService";
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;
        }

        protected override void OnStop()
        {
            stateTimer.Dispose();
            base.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            timerDelegate = test.DoIt;
            stateTimer = new Timer(timerDelegate, null, 1000, 1000);
            base.OnStart(args);

        }

        public static void Main()
        {
            Run(new RemindService());
        }

        private void InitializeComponent()
        {
            // 
            // RemindService
            // 
            ServiceName = "RemindService";

        }
    }
}
