using System;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;

namespace ServiceWrapper
{
    [RunInstaller(true)]
    public class ProjectServiceInstaller : Installer
    {
        ServiceInstaller _installer;
        ServiceProcessInstaller _serviceProcessInstaller;

        public ProjectServiceInstaller()
        {
            Assembly serviceAssembly = Assembly.GetAssembly(typeof(ProjectServiceInstaller));
            Configuration config = ConfigurationManager.OpenExeConfiguration(serviceAssembly.Location);
            KeyValueConfigurationCollection settings = config.AppSettings.Settings;


            _serviceProcessInstaller = new ServiceProcessInstaller();
            _serviceProcessInstaller.Username = null;
            _serviceProcessInstaller.Password = null;
            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            ServiceAccount serviceAccount;
            if (Enum.TryParse<ServiceAccount>(ConfigurationManager.AppSettings["RunMode"], out serviceAccount))
            {
                _serviceProcessInstaller.Account = serviceAccount;
                if (serviceAccount == ServiceAccount.User)
                {
                    _serviceProcessInstaller.Username = settings["Username"].Value;
                    _serviceProcessInstaller.Password = settings["Password"].Value;
                }
            }

            _installer = new ServiceInstaller();

            EventLog.WriteEntry("ServiceWrapper",
                string.Format("Install new Service instance, Displayname={0}, ServiceName={1}",
                    settings["DisplayName"].Value, settings["ServiceName"].Value),
                EventLogEntryType.Information);

            _installer.ServiceName = settings["ServiceName"].Value;
            _installer.DisplayName = settings["DisplayName"].Value;
            _installer.Description = settings["Description"].Value;

            Installer[] installer = new Installer[2];
            installer[0] = _serviceProcessInstaller;
            installer[1] = _installer;

            base.Installers.AddRange(installer);
        }
    }
}