namespace CPSCBS
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
            System.ServiceProcess.ServiceProcessInstaller CPSCBSServOutwardRVProcessInstaller;
            this.CPSCBSOutwardRVInstaller = new System.ServiceProcess.ServiceInstaller();
            CPSCBSServOutwardRVProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // CPSCBSServOutwardRVProcessInstaller
            // 
            CPSCBSServOutwardRVProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            CPSCBSServOutwardRVProcessInstaller.Password = null;
            CPSCBSServOutwardRVProcessInstaller.Username = null;
            // 
            // CPSCBSOutwardRVInstaller
            // 
            this.CPSCBSOutwardRVInstaller.Description = "Vital Process of Flora CPS Outward(RV) to communicate with CBS";
            this.CPSCBSOutwardRVInstaller.DisplayName = "Flora CBS Outward(RV) Integration Service";
            this.CPSCBSOutwardRVInstaller.ServiceName = "FloraCPSCBSOutwardRV";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            CPSCBSServOutwardRVProcessInstaller,
            this.CPSCBSOutwardRVInstaller});

        }

        #endregion
        private System.ServiceProcess.ServiceInstaller CPSCBSOutwardRVInstaller;
    }
}