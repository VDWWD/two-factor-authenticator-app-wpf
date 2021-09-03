using ControlzEx;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwoFactor.Classes
{
    //icons in vector: https://materialdesignicons.com

    public class ResourceController
    {
        public static Style StyleTextBoxNormal
        {
            get
            {
                return (Style)Application.Current.FindResource("TextBox_Normal");
            }
        }
        public static Style StyleTextBoxError
        {
            get
            {
                return (Style)Application.Current.FindResource("TextBox_Error");
            }
        }
        public static Style StyleTextBoxDarkMode
        {
            get
            {
                return (Style)Application.Current.FindResource("TextBox_Normal_DarkMode");
            }
        }

        public static Brush BrushWhite
        {
            get
            {
                return (Brush)Application.Current.FindResource("White");
            }
        }
        public static Brush BrushBlack
        {
            get
            {
                return (Brush)Application.Current.FindResource("Black");
            }
        }
        public static Brush BrushDarkModeText
        {
            get
            {
                return (Brush)Application.Current.FindResource("DarkMode_Text");
            }
        }
        public static Brush BrushDarkModeBackground
        {
            get
            {
                return (Brush)Application.Current.FindResource("DarkMode_Background");
            }
        }
        public static Brush BrushGray
        {
            get
            {
                return (Brush)Application.Current.FindResource("TekstKleur");
            }
        }


        /// <summary>
        /// Loads an image from the included resource files
        /// </summary>
        /// <param name="image">The image name as string</param>
        /// <returns>Bitmap</returns>
        public static BitmapImage GetImageFromResource(string image)
        {
            var bitmap = new BitmapImage();

            using (var stream = GetStreamFromResource(image))
            {
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            return bitmap;
        }


        /// <summary>
        /// Load a stream from the included resources
        /// </summary>
        /// <param name="filename">The filename name as string</param>
        /// <returns>Stream</returns>
        public static Stream GetStreamFromResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(string.Format("{0}.Resources.{1}", assembly.GetName().Name, filename));
        }


        /// <summary>
        /// Creates a taskbaricon context menu item with an icon and text
        /// </summary>
        /// <param name="icon">The icon name in Enum</param>
        /// <param name="text">The text in the menu item</param>
        /// <param name="handler">The event that is trigger when clicking the menu item</param>
        /// <returns>MenuItem</returns>
        public static MenuItem CreateContextMenuItem(Enums.Icon icon, string text, RoutedEventHandler handler)
        {
            var path = IconController.GetIcon(icon, BrushWhite);

            var viewBox = new Viewbox()
            {
                Width = 22,
                Height = 22,
                Margin = new Thickness(0, 1, 0, 0)
            };

            var canvas = new Canvas()
            {
                Width = 24,
                Height = 24
            };

            canvas.Children.Add(path);
            viewBox.Child = canvas;

            var item = new MenuItem()
            {
                Header = text,
                Icon = viewBox
            };

            item.Click += handler;

            return item;
        }
    }
}
