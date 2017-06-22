using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using  pi = PharmaceuticalInformation;

namespace ServerOfSystem
{
    public partial class Server : ServiceBase
    {

        #region ' Fields '

        private pi.Server.Server _ServerOfPharmaceuticalInformation;
        // =             new PharmaceuticalInformation.Server();

        #endregion

        #region ' Designer '

        public Server()
        {
            //
            InitializeComponent();
            //
            _ServerOfPharmaceuticalInformation = new pi.Server.Server();
        }

        #endregion

        #region ' Management Of Service '

        protected override void OnStart(string[] args)
        {
            //
            _ServerOfPharmaceuticalInformation.StartingOfServer();
        }

        protected override void OnStop()
        {
            //
            _ServerOfPharmaceuticalInformation.StopingOfServer();
        }

        #endregion

    }
}