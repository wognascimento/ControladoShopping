namespace ControladoShopping
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTU4NUAzMjM3MkUzMTJFMzluT08wbzRnYm4zUlFDOVRzWVpYbUtuSEl0aUhTZmNMYjQxekhrV0NVRnlzPQ==");

            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
