using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Memory_Trainer
{
    class ButtonExtended
    {
        private Button MyButton { get; set; }
        private string ID { get; set; }
        private MainWindow MyWindow { get; set; }
        private readonly ColorAnimation colorAnimation;
        private readonly Storyboard storyboard;
        private Brush defaultColor = Brushes.LightGray;
        private readonly Brush correctColor = Brushes.Green;
        private readonly Brush failColor = Brushes.Red;

        public ButtonExtended(string xID, string yID, Button button, MainWindow mainWindow, bool isOdd)
        {
            if (isOdd)
                defaultColor = Brushes.DarkGray;
            ID = xID + yID;
            MyButton = button;
            MyButton.Click += ReturnID;
            MyWindow = mainWindow;
            button.Background = defaultColor;

            colorAnimation = new ColorAnimation()
            {
                Duration = TimeSpan.FromSeconds(5),
                To = ((SolidColorBrush)defaultColor).Color
            };
            storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            Storyboard.SetTarget(colorAnimation, MyButton);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
        }

        private void ReturnID(object sender, RoutedEventArgs e)
        {

            if (ID == MyWindow.Target)
            {
                SetColor(correctColor);
                MyWindow.L_Output.Content = "Correct";
            }
            else
            {
                SetColor(failColor);
                MyWindow.L_Output.Content = "Fail";
            }
            storyboard.Begin();
            MyWindow.RandomizeTarget();
        }

        private void SetColor(Brush color)
        {
            colorAnimation.From = ((SolidColorBrush)color).Color;
            MyButton.Background = color;
            MyWindow.L_Output.Foreground = color;
        }
    }
}
