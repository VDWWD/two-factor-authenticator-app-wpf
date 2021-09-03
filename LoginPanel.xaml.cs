using OtpNet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TwoFactor
{
    // https://github.com/panthernet/XamlRadialProgressBar

    public partial class LoginPanel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Classes.Setting.Login Login { get; set; }
        public bool NoBubble { get; set; }
        public EditLogin EditLogin { get; set; }
        public decimal TickSize { get; set; }


        private double _percentage;
        public double percentage
        {
            get => _percentage;
            set { _percentage = value; OnPropertyChanged(); }
        }

        private string _number;
        public string number
        {
            get => _number;
            set { _number = value; OnPropertyChanged(); }
        }

        private string _name;
        public string name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _color_text;
        public SolidColorBrush color_text
        {
            get => _color_text;
            set { _color_text = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _color_number;
        public SolidColorBrush color_number
        {
            get => _color_number;
            set { _color_number = value; OnPropertyChanged(); }
        }


        public LoginPanel(Classes.Setting.Login log_in, bool is_last, DateTime date)
        {
            InitializeComponent();

            //some variables
            DataContext = this;
            Login = log_in;
            Loaded += (sender, args) => InitializeTimers();
            TickSize = 3.35m;

            //1 second = 1.666 percent
            var seconds = date.Second;
            if (seconds > 31)
                seconds = seconds - 30;

            //calculate percentage
            percentage = Convert.ToInt32(seconds * TickSize);

            _number = GenerateCode();
            _name = Login.name;

            //styles and texts
            txt_copied.Text = Localizer.GetLocalized("mainwindow-copied");
            _color_text = (SolidColorBrush)Classes.ResourceController.BrushGray;
            _color_number = new SolidColorBrush(Color.FromRgb(55, 69, 106));
            horizontal_line.Fill = new SolidColorBrush(Color.FromRgb(218, 218, 218));

            //on last item hide separator
            if (is_last)
            {
                horizontal_line.Fill = Classes.ResourceController.BrushWhite;
            }

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                _color_text = new SolidColorBrush(Color.FromRgb(206, 206, 206));
                _color_number = new SolidColorBrush(Color.FromRgb(137, 155, 205));

                horizontal_line.Fill = new SolidColorBrush(Color.FromRgb(64, 64, 64));

                //on last item hide separator
                if (is_last)
                {
                    horizontal_line.Fill = Classes.ResourceController.BrushDarkModeBackground;
                }
            }
        }

        /// <summary>
        /// Creates a timer with a 1 second tick
        /// </summary>
        private void InitializeTimers()
        {
            //1 second timer
            var timer = new Timer(1000);

            timer.Elapsed += (sender, args) =>
            {
                percentage += (double)TickSize;

                //if full cirlce then clear and generate new code
                if (percentage >= 100)
                {
                    percentage = 0;
                    number = GenerateCode();
                }
            };

            timer.Start();
        }


        /// <summary>
        /// Copies the 6 digit code to the Windows clipboard
        /// </summary>
        private void Button_toclipboard_Click(object sender, RoutedEventArgs e)
        {
            if (NoBubble)
            {
                name = Login.name;
                NoBubble = false;

                //if the login was deleted from the edit login window then hide it
                var login = MainWindow.Settings.logins.Where(x => x.guid == Login.guid).FirstOrDefault();
                if (login == null)
                {
                    this.Visibility = Visibility.Collapsed;
                }

                return;
            }

            //update data and save
            Login.date_used = DateTime.Now;
            Login.used++;
            MainWindow.Settings.Save();

            //copy code to clipboard
            Clipboard.SetText(number);

            //show the copied message
            ShowMessage();
        }


        /// <summary>
        /// Opens the settings window for the login
        /// </summary>
        private void Button_settings_Click(object sender, RoutedEventArgs e)
        {
            NoBubble = true;

            EditLogin = new EditLogin(Login)
            {
                Owner = Application.Current.MainWindow
            };

            EditLogin.ShowDialog();
        }


        /// <summary>
        /// Show the copied to clipboard message on top of the login
        /// </summary>
        private void ShowMessage()
        {
            txt_copied_container.Visibility = Visibility.Visible;

            //create the animation
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(3),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            //create a storyboard to fade out the message
            var storyboard = new Storyboard();

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, txt_copied_container);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { txt_copied_container.Visibility = Visibility.Hidden; };
            storyboard.Begin();
        }


        /// <summary>
        /// calculates the 6 digit code
        /// </summary>
        /// <returns>6 digit code as a string</returns>
        public string GenerateCode()
        {
            var secretKey = Base32Encoding.ToBytes(Login.secret);
            var totp = new Totp(secretKey);
            return totp.ComputeTotp();
        }


        /// <summary>
        /// OnPropertyChanged handler
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
