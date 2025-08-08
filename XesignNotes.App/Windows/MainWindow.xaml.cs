using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XeZrunner.UI.Popups;
using XeZrunner.UI.Theming;

namespace XesignNotes.App.Windows
{
    public partial class MainWindow : Window
    {

        #region Win32 Acrylic

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public uint AccentFlags;
            public uint GradientColor;
            public uint AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private uint _blurOpacity;
        public double BlurOpacity
        {
            get { return _blurOpacity; }
            set { _blurOpacity = (uint)value; EnableBlur(); }
        }

        private uint _blurBackgroundColor = 0x990000; /* BGR color format */

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            accent.GradientColor = (_blurOpacity << 24) | (_blurBackgroundColor & 0xFFFFFF);

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        #endregion

        public ThemeManager ThemeManager;
        Configuration.ConfigurationManager ConfigurationManager = new Configuration.ConfigurationManager();

        public MainWindow()
        {
            InitializeComponent();

            ThemeManager = new ThemeManager(Application.Current.Resources);
            ThemeManager.ConfigChanged += ThemeManager_ConfigChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //EnableBlur();

            SwitchToView("Home");

            ConfigurationManager.CheckConfiguration();

            LoadUserConfiguration();
        }

        public void LoadUserConfiguration()
        {
            List<Configuration.Configuration> UserConfig = ConfigurationManager.GetUserConfiguration();
            foreach (Configuration.Configuration config in UserConfig)
            {
                if (config.Property == "Theme")
                    ThemeManager.Config_SetTheme(ThemeManager.GetThemeFromString(config.Value.ToString()));

                if (config.Property == "Accent")
                    ThemeManager.Config_SetAccent(ThemeManager.GetAccentFromString(config.Value.ToString()));

                if (config.Property == "RevealFX")
                {
                    bool value = (bool)config.Value;
                    if (value)
                        XeZrunner.UI.Configuration.Config.Default.controlfx = "Reveal";
                    else
                        XeZrunner.UI.Configuration.Config.Default.controlfx = "P";
                }

                if (config.Property == "AcrylicFX")
                    fallback_Background.Opacity = (bool)config.Value ? 0 : 1;
            }
        }

        public async void ShowTextContentDialog(string Title, string Text)
        {
            ContentDialog dialog = new ContentDialog() { Title = Title, Content = Text, PrimaryButtonText = "OK", SecondaryButtonText = "" };
            await contentdialogHost.ShowDialogAsync(dialog);
        }

        List<Page> _pageMemory = new List<Page>();
        public async void SwitchToView(string pageName)
        {
            Page page = null;

            // get the page from memory first:
            foreach (Page _memPage in _pageMemory)
            {
                if (_memPage.Title == pageName)
                    page = _memPage;
            }

            if (page == null) // if the page wasn't in memory:
            {
                Page pageToAdd = GetPageObject(pageName);

                if (pageToAdd != null)
                    _pageMemory.Add(pageToAdd); // create a new one and add to memory
                else
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "SwitchToView(): error",
                        Content = "Could not switch to the requested page: \n\nThe requested page '" + pageName + "' is invalid.",
                        PrimaryButtonText = "OK",
                        SecondaryButtonText = ""
                    };
                    await contentdialogHost.ShowDialogAsync(dialog);

                    return;
                }

                SwitchToView(pageName);
                return;
            }
            else // if the page was in memory:
            {
                frameContainer.Children.Add(CreateFrameWithPage(pageName, page)); // create the Frame that will hold the requested page
                SwitchToFrame(pageName); // switch to the Frame holding the requested page
            }
        }

        Frame CreateFrameWithPage(string pageName, Page pageObject)
        {
            return new Frame() { Name = pageName + "_Frame", Content = pageObject };
        }

        void SwitchToFrame(string pageName)
        {
            foreach (Frame frame in frameContainer.Children)
            {
                if (frame.Name == pageName + "_Frame")
                {
                    if (frame.Name == "Home_Frame")
                    {
                        frame.Visibility = Visibility.Visible;
                    }
                    else
                        AnimInObject(frame);
                }
                else
                    AnimOutObject(frame);
            }
        }

        void AnimInObject(FrameworkElement element)
        {
            element.Visibility = Visibility.Visible;

            DoubleAnimation animation = new DoubleAnimation(this.ActualHeight, 0, TimeSpan.FromSeconds(1));
            PowerEase ease = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 8 };
            animation.EasingFunction = ease;
            TranslateTransform translate = new TranslateTransform();
            element.RenderTransform = translate;
            translate.BeginAnimation(TranslateTransform.YProperty, animation);

            DoubleAnimation anim_fade = new DoubleAnimation(1, TimeSpan.FromSeconds(.3));
            element.BeginAnimation(OpacityProperty, anim_fade);
        }

        async void AnimOutObject(FrameworkElement element)
        {
            DoubleAnimation animation = new DoubleAnimation(0, this.ActualHeight, TimeSpan.FromSeconds(1));
            PowerEase ease = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 8 };
            animation.EasingFunction = ease;
            TranslateTransform translate = new TranslateTransform();
            element.RenderTransform = translate;
            translate.BeginAnimation(TranslateTransform.YProperty, animation);

            DoubleAnimation anim_fade = new DoubleAnimation(0, TimeSpan.FromSeconds(.3));
            element.BeginAnimation(OpacityProperty, anim_fade);

            await Task.Delay(TimeSpan.FromSeconds(.5));
            element.Visibility = Visibility.Hidden;
        }

        Page GetPageObject(string pageName)
        {
            switch (pageName)
            {
                default: return null;
                case "Home": return new Home.MainPage();
                case "Settings": return new Settings.SettingsPage();
            }
        }

        RenderTargetBitmap Screenshot(FrameworkElement element)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(element);

            return renderTargetBitmap;
        }

        private async void ThemeManager_ConfigChanged(object sender, EventArgs e)
        {
            themechangeImage.Source = Screenshot(this);

            themechangeImage.Visibility = Visibility.Visible;

            DoubleAnimation anim = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(.3));
            themechangeImage.BeginAnimation(OpacityProperty, anim);

            await Task.Delay(TimeSpan.FromSeconds(.3));
            themechangeImage.Visibility = Visibility.Hidden;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
#if false
            if (e.Key == Key.Q)
                ThemeManager.Config_SetTheme(ThemeManager.Theme.Light);

            if (e.Key == Key.E)
                ThemeManager.Config_SetTheme(ThemeManager.Theme.Dark);
#endif
        }
    }
}
