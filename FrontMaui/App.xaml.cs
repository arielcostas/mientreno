namespace FrontMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            if (window != null)
            {
                window.MinimumWidth = 350;
                window.MinimumHeight = 300;

                window.Title = "Cosa de tareas";

            }
            return window;
        }
    }
}