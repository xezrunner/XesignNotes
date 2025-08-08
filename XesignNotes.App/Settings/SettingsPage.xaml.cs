using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XeZrunner.UI.Popups;

namespace XesignNotes.App.Settings
{
    public partial class SettingsPage : Page
    {
        Windows.MainWindow mainwindow = (Windows.MainWindow)Application.Current.MainWindow;
        Engine.NoteManager NoteManager = new Engine.NoteManager();
        Configuration.ConfigurationManager ConfigManager = new Configuration.ConfigurationManager();

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetUserConfig();
        }

        void GetApplicationConfig()
        {
            List<Configuration.Configuration> AppConfig = ConfigManager.GetApplicationConfiguration();

            foreach (Configuration.Configuration config in AppConfig)
            {
                if (config.Property == "IsThemingEnabled")
                    darkmodeSwitch.Visibility = (bool)config.Value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        void GetUserConfig()
        {
            List<Configuration.Configuration> UserConfig = ConfigManager.GetUserConfiguration();

            foreach (Configuration.Configuration config in UserConfig)
            {
                if (config.Property == "Theme")
                    darkmodeSwitch.IsActive = config.Value.ToString() == "Light" ? false : true;
            }
        }

        public event MouseButtonEventHandler BackButtonClick;

        private void appbar_backButton_Click(object sender, RoutedEventArgs e)
        {
            //mainwindow.SwitchToView("Home");
            BackButtonClick?.Invoke(this, null);
        }

        private void darkmodeSwitch_IsActiveChanged(object sender, EventArgs e)
        {
            if (darkmodeSwitch.IsActive)
                mainwindow.ThemeManager.Config_SetTheme(XeZrunner.UI.Theming.ThemeManager.Theme.Dark);
            else
                mainwindow.ThemeManager.Config_SetTheme(XeZrunner.UI.Theming.ThemeManager.Theme.Light);

            ConfigManager.ChangeUserConfiguration("Theme", darkmodeSwitch.IsActive == false ? "Light" : "Dark");

            //mainwindow.ShowTextContentDialog("", ConfigManager.User_GetValue("Theme").ToString());
        }

        private void debugmodeSwitch_IsActiveChanged(object sender, EventArgs e)
        {
            mainwindow.SwitchToView("DebugEnroll");
            if (debugmodeSwitch.IsActive)
            {
                debugmodeSwitch.IsActive = false;
                return;
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainwindow.DragMove();
        }

        private async void deleteAllNotesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Are you sure you want to delete all user notes?",
                Content = "This will delete every single note you have taken, and they cannot be restored!",
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel"
            };

            if (await mainwindow.contentdialogHost.ShowDialogAsync(dialog) == ContentDialogHost.ContentDialogResult.Primary)
                NoteManager.DeleteAllNotes();
        }

        private void clearConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.ClearConfiguration();
        }

        private void debugmodeSwitch_IsActiveChanged(object sender, bool e)
        {

        }
    }
}
