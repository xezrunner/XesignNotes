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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XeZrunner.UI.Controls;
using XeZrunner.UI.Controls.Buttons;
using XeZrunner.UI.Popups;

namespace XesignNotes.App.Home
{
    public partial class MainPage : Page
    {
        Windows.MainWindow mainwindow = (Windows.MainWindow)Application.Current.MainWindow;

        Engine.NoteManager NoteManager = new Engine.NoteManager();

        public MainPage()
        {
            InitializeComponent();

            autosaveTimer.Tick += AutosaveTimer_Tick;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetUserNotes();
        }

        public List<string> _noteList;

        NavigationMenu _noteMenu;

        void GetUserNotes()
        {
            textbox.Document = new FlowDocument();

            _noteList = NoteManager.GetNotes();

            _noteMenu = new NavigationMenu();
            _noteMenu.SelectionChanged += noteMenuControl_SelectionChanged;
            foreach (string note in _noteList)
            {
                NavMenuItem button = new NavMenuItem() { Text = note, Icon = "" };
                button.PreviewMouseRightButtonUp += NoteListItems_PreviewMouseRightButtonUp;

                _noteMenu.Items.Add(button);
            }

            menuControl.Middle = _noteMenu;
        }

        string _NoteName;

        private void noteMenuControl_SelectionChanged(object sender, EventArgs e)
        {
            NavMenuItem clickedButton = (NavMenuItem)sender;
            string noteName = clickedButton.Text;

            NoteManager.LoadDocument(noteName, textbox);
            _NoteName = noteName;
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainwindow.DragMove();
        }

        private void menu_settingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (settings_Frame.Content == null)
            {
                Settings.SettingsPage page = new Settings.SettingsPage();
                page.BackButtonClick += settings_Area_Background_MouseLeftButtonUp;

                settings_Frame.Content = page;
            }

            settings_Area.Visibility = Visibility.Visible;
            DoubleAnimation animation_fadein = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation_fadein_tint = new DoubleAnimation(0, 0.03, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation = new DoubleAnimation(0, 25, TimeSpan.FromSeconds(.5));
            animation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
            settings_Area_Tint.BeginAnimation(OpacityProperty, animation_fadein_tint);
            settings_Area_BlurEffect.BeginAnimation(BlurBitmapEffect.RadiusProperty, animation);
            settings_Frame.BeginAnimation(OpacityProperty, animation_fadein);
        }

        private async void settings_Area_Background_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation animation_fadeout = new DoubleAnimation(0, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation_fadeout_tint = new DoubleAnimation(0, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(.5));
            animation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
            settings_Area_Tint.BeginAnimation(OpacityProperty, animation_fadeout_tint);
            settings_Area_BlurEffect.BeginAnimation(BlurBitmapEffect.RadiusProperty, animation);
            settings_Frame.BeginAnimation(OpacityProperty, animation_fadeout);

            await Task.Delay(TimeSpan.FromSeconds(.5));
            settings_Area.Visibility = Visibility.Hidden;

            GetUserNotes();
        }

        private async void newNoteButton_Click(object sender, RoutedEventArgs e)
        {
        newNoteDialog:
            ContentDialog dialog = new ContentDialog() { Title = "Create new note", PrimaryButtonText = "Create", SecondaryButtonText = "Cancel" };
            TextField textfield = new TextField() { Title = "Title: " };
            dialog.Content = textfield;
            textfield.Focus();
            if (await mainwindow.contentdialogHost.ShowDialogAsync(dialog) == ContentDialogHost.ContentDialogResult.Primary)
            {
                string title = textfield.Text.Trim();
                if (string.IsNullOrEmpty(title)) {
                    await mainwindow.contentdialogHost.ShowDialogAsync(new ContentDialog() { 
                        Title = "Please enter a title.",
                        Content = null
                    });
                }

                NoteManager.CreateNoteFile(textfield.Text, Engine.NoteColor.Monochrome);
                GetUserNotes();
            }
        }

        bool _isAutomaticSave;
        bool IsAutomaticSave
        {
            get { return _isAutomaticSave; }
            set
            {
                _isAutomaticSave = value;

                if (value)
                { /* hide manual save button */ }
                else
                { /* show manual save button */ }
            }
        }

        void DoAutomaticSave()
        {
            NoteManager.SaveDocument(_NoteName, textbox);
        }

        DispatcherTimer autosaveTimer = new DispatcherTimer();

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Random random = new Random();

            autosaveTimer.Stop();
            autosaveTimer.Interval = TimeSpan.FromSeconds(random.Next(1, 3));
            autosaveTimer.Start();
        }

        private void AutosaveTimer_Tick(object sender, EventArgs e)
        {
            autosaveTimer.Stop();

            if (_NoteName == null)
                return;

            TextRange range = new TextRange(textbox.Document.ContentStart, textbox.Document.ContentEnd);
            if (range.Text.Length < 1000)
            { IsAutomaticSave = true; DoAutomaticSave(); }
            else
                IsAutomaticSave = false;
        }

        private void menu_searchBar_Click(object sender, RoutedEventArgs e)
        {
            searchOverlay.Visibility = Visibility.Visible;
            DoubleAnimation animation_fadein = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation_fadein_tint = new DoubleAnimation(0, 0.56, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation = new DoubleAnimation(0, 25, TimeSpan.FromSeconds(.5));
            animation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
            searchOverlay_BlurEffect.BeginAnimation(BlurBitmapEffect.RadiusProperty, animation);
            searchOverlay_Panel.BeginAnimation(OpacityProperty, animation_fadein);
            searchOverlay_BackgroundTint.BeginAnimation(OpacityProperty, animation_fadein_tint);

            search_overlay_TextField.Focus();
        }

        private async void searchOverlay_Background_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation animation_fadeout = new DoubleAnimation(0, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(.5));
            animation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
            searchOverlay_BlurEffect.BeginAnimation(BlurBitmapEffect.RadiusProperty, animation);
            searchOverlay_Panel.BeginAnimation(OpacityProperty, animation_fadeout);
            searchOverlay_BackgroundTint.BeginAnimation(OpacityProperty, animation_fadeout);

            await Task.Delay(TimeSpan.FromSeconds(.5));
            searchOverlay.Visibility = Visibility.Hidden;
        }

        private void NoteListItems_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // set location of popup
            NavMenuItem button = (NavMenuItem)sender;
            Point point = button.TranslatePoint(new Point(), this);

            noteList_MainPopup.Margin = new Thickness(0, point.Y, 0, 0);

            // set name of note
            noteList_Popup_NoteButton.Text = button.Text;
            noteList_Popup_NoteButton.IsActive = button.IsActive;

            // make popup visible

            noteList_PopupArea.Visibility = Visibility.Visible;

            DoubleAnimation animation_fadein = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation_radius = new DoubleAnimation(0, 25, TimeSpan.FromSeconds(.5));
            animation_radius.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };

            noteList_MainPopup.BeginAnimation(OpacityProperty, animation_fadein);
            noteList_PopupArea_BlurEffect.BeginAnimation(BlurBitmapEffect.RadiusProperty, animation_radius);
        }

        private async void Rectangle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // hide popup
            DoubleAnimation animation_fadeout = new DoubleAnimation(0, TimeSpan.FromSeconds(.3));
            DoubleAnimation animation_radius = new DoubleAnimation(0, TimeSpan.FromSeconds(.5));
            animation_radius.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };

            noteList_MainPopup.BeginAnimation(OpacityProperty, animation_fadeout);
            noteList_PopupArea_BlurEffect.BeginAnimation(BlurBitmapEffect.RadiusProperty, animation_radius);

            await Task.Delay(TimeSpan.FromSeconds(.5));
            noteList_PopupArea.Visibility = Visibility.Hidden;
        }

        private async void noteList_Popup_DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Delete note",
                Content = "Are you sure you want to delete this note?\nWarning: this action cannot be reversed!",
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel"
            };

            if (await mainwindow.contentdialogHost.ShowDialogAsync(dialog) == ContentDialogHost.ContentDialogResult.Primary)
            {
                NoteManager.DeleteDocument(noteList_Popup_NoteButton.Text);
                GetUserNotes();

                Rectangle_PreviewMouseLeftButtonUp(null, null);
            }
        }

        private void noteList_Popup_ChangeColorButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Change note color",
                Content = "Under construction",
                PrimaryButtonText = "OK"
            };

            mainwindow.contentdialogHost.ShowDialog(dialog);
        }

        private void noteList_Popup_DebugInfoButton_Click(object sender, RoutedEventArgs e)
        {
            string debuginfo = "debug info";

            mainwindow.ShowTextContentDialog("Debug info", debuginfo);
        }

        private void noteList_Popup_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Rectangle_PreviewMouseLeftButtonUp(null, null);
        }

        private void searchOverlay_backButton_Click(object sender, RoutedEventArgs e)
        {
            searchOverlay_Background_MouseLeftButtonUp(null, null);
        }

        private void search_overlay_TextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> searchItems = NoteManager.SearchFiles(search_overlay_TextField.Text);

            searchList.Children.Clear();

            foreach (string sItem in searchItems)
            {
                NavMenuItem button = new NavMenuItem() { Text = sItem };
                button.Click += search_overlay_item_Click;

                searchList.Children.Add(button);
            }
        }

        void search_overlay_item_Click(object sender, RoutedEventArgs e)
        {
            NavMenuItem sButton = (NavMenuItem)sender;
            searchOverlay_Background_MouseLeftButtonUp(this, null);

            noteMenuControl_SelectionChanged(sButton, null);
        }

        private void Menu_toggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (menuControl.State == MenuControl.MenuState.Open)
            {
                menu_toggleButton.Icon = "\ue700";
                menuControl.Close();
            }
            else
            {
                menu_toggleButton.Icon = "\ue76b";
                menuControl.Open();
            }
        }
    }
}
