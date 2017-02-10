namespace DemoWinService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller_DemoWinService = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller_DemoWinService = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller_DemoWinService
            // 
            this.serviceProcessInstaller_DemoWinService.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstaller_DemoWinService.Password = null;
            this.serviceProcessInstaller_DemoWinService.Username = null;
            // 
            // serviceInstaller_DemoWinService
            // 
            this.serviceInstaller_DemoWinService.DisplayName = "DemoWinService";
            this.serviceInstaller_DemoWinService.ServiceName = "DemoWinService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller_DemoWinService,
            this.serviceInstaller_DemoWinService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller_DemoWinService;
        private System.ServiceProcess.ServiceInstaller serviceInstaller_DemoWinService;
    }
}