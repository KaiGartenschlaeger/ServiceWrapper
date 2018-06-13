using System.Diagnostics;
using System.ServiceProcess;

namespace ServiceWrapper
{
    public class HostService : ServiceBase
    {
        private HostEngine _engine;

        public HostService()
        {
            _engine = new HostEngine(false);
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Service Wrapper", "Service started",
                EventLogEntryType.Information);

            _engine.Start();
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Service Wrapper", "Service stopped",
                EventLogEntryType.Information);

            _engine.Stop();
        }

        protected override void OnShutdown()
        {
            EventLog.WriteEntry("Service Wrapper", "Windows shutdown, Service stopped",
                EventLogEntryType.Information);

            _engine.Stop();
        }
    }
}