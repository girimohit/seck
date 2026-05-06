using System.Windows;
using System.Windows.Input;
using SecureAppLocker.ViewModels;

namespace SecureAppLocker.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        // Drag to move the borderless window
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}



// // using System.Text;
// // using System.Windows;
// // using System.Windows.Controls;
// // using System.Windows.Data;
// // using System.Windows.Documents;
// // using System.Windows.Input;
// // using System.Windows.Media;
// // using System.Windows.Media.Imaging;
// // using System.Windows.Navigation;
// // using System.Windows.Shapes;

// // namespace SecLock;
// // using SecureAppLocker.ViewModels;

// // /// <summary>
// // /// Interaction logic for MainWindow.xaml
// // /// </summary>
// // public partial class MainWindow : Window
// // {
// //     public MainWindow()
// //     {
// //         InitializeComponent();
// //         DataContext = new MainViewModel();
// //     }
// // }

// using System;
// using System.Windows;
// using System.Windows.Forms;
// using SecureAppLocker.ViewModels;
// using Application = System.Windows.Application;

// namespace SecureAppLocker.Views
// {
//     public partial class MainWindow : Window
//     {
//         private NotifyIcon _trayIcon = null!;

//         public MainWindow()
//         {
//             InitializeComponent();
//             DataContext = new MainViewModel();
//             InitializeTray();
//         }

//         private void InitializeTray()
//         {
//             _trayIcon = new NotifyIcon
//             {
//                 Icon = System.Drawing.SystemIcons.Shield,
//                 Visible = true,
//                 Text = "Secure App Locker"
//             };

//             var menu = new ContextMenuStrip();
//             menu.Items.Add("Open", null, (s, e) => ShowWindow());
//             menu.Items.Add("Exit", null, (s, e) => ExitApp());

//             _trayIcon.ContextMenuStrip = menu;

//             _trayIcon.DoubleClick += (s, e) => ShowWindow();
//         }

//         private void ShowWindow()
//         {
//             this.Show();
//             this.WindowState = WindowState.Normal;
//         }

//         private void ExitApp()
//         {
//             _trayIcon.Visible = false;
//             Application.Current.Shutdown();
//         }

//         protected override void OnStateChanged(EventArgs e)
//         {
//             base.OnStateChanged(e);

//             if (WindowState == WindowState.Minimized)
//             {
//                 Hide();
//             }
//         }

//         protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
//         {
//             e.Cancel = true;
//             Hide();
//         }


//     }
// }