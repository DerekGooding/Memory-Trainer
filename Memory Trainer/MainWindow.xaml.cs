using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.IO;
using System.Linq;
using System.Windows.Media.Animation;
using System.Media;

namespace Memory_Trainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly int size = 8;
        public MainWindow()
        {
            InitializeComponent();
            PrepareBoard();
            GetVoice();
            RandomizeTarget();
            PrepareTimerBar();
        }
        void PrepareBoard()
        {
            for (int i = size; i >= 1; i--)
                SP_Grid.Children.Add(CreateRow(i));
        }

        StackPanel CreateRow(int yValue)
        {
            var output = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            for (int i = 1; i <= size; i++)
                output.Children.Add(CreateCell(i, yValue));
            return output;
        }

        Button CreateCell(int xValue, int yValue)
        {
            var myWidth = G_Grid.Width / size;
            var myHeight = G_Grid.Height / size;
            var output = new Button
            {
                Height = myHeight,
                Width = myWidth
            };
            bool isOdd = false;
            if ((xValue + yValue) % 2 == 0) isOdd = true;
            new ButtonExtended(ConvertToLetter(xValue), yValue.ToString(), output, this, isOdd);
            return output;
        }

        string ConvertToLetter(int input)
        {
            var output = 'A';
            for (int i = 0; i < input - 1; i++)
            {
                output++;
            }
            return output.ToString();
        }
        string target;
        private readonly Random rnd = new Random();
        public string Target { get => target;}
        public void RandomizeTarget()
        {
            target = ConvertToLetter(rnd.Next(1, size + 1)) + rnd.Next(1, size + 1);
            L_Target.Content = target;
            TextToSpeech(target);
        }

        string CurrentVoice;
        private void GetVoice()
        {
            var synthesizer = new SpeechSynthesizer();
            var voiceCollection = synthesizer.GetInstalledVoices();
            CurrentVoice = voiceCollection.First().VoiceInfo.Name;
        }

        private void TextToSpeech(string message)
        {
            string[] seperator = { "A" };
            if(message.Contains("A"))
            {
                var temp = message.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                message = "A. " + temp[0];
            }
            Thread thread = new Thread(() =>
            {
                var synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.SelectVoice(CurrentVoice);
                synthesizer.Speak(message);
            });
            thread.Start();
        }

        public DoubleAnimation doubleAnimation;
        public Storyboard storyboard;
        private void PrepareTimerBar()
        {
            doubleAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(5),
                To = 200,
                From = 0
            };
            storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, FillAmount);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(HeightProperty));
            storyboard.Completed += HandleComplete;
            storyboard.Begin();
            
        }

        private void HandleComplete(object sender, EventArgs e)
        {
            SystemSounds.Hand.Play();
            RandomizeTarget();
            storyboard.Stop();
            storyboard.Begin();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => FillPercent(e.NewValue / 10);

        private void FillPercent(double value) => FillAmount.Height = 200 - (value * 200);
    }
}
