using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace ImportaCHICService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            //serviceProcessInstaller1.Username = "hyline\administrador";
            //serviceProcessInstaller1.Password = "hyline2012";
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
