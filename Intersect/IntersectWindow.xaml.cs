using Nayoh.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nayoh.Intersect
{
    public partial class Core : Window
    {
        private readonly SolidColorBrush darkRed = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#660000"));
        private readonly SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private readonly string Exe_Path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly Nayoh.Intersect.Files files = new Nayoh.Intersect.Files();
        private Dictionary<string, object> conf = new Dictionary<string, object>();
        private string intersectMessage, welcomeMessage, intersectClipboard;
        private int pos, historyIndex, target, intersectImgDelay = 0;
        private readonly List<string> history = new List<string>();
        private List<Quiz> quiz = new List<Quiz>();
        private bool failure, skip, debug;

        public Core()
        {
            InitializeComponent();
            Answer_TextBox.SelectionChanged += (sender, e) => MoveCustomCaret();
            Answer_TextBox.LostFocus += (sender, e) => Caret.Visibility = Visibility.Collapsed;
            Answer_TextBox.GotFocus += (sender, e) => Caret.Visibility = Visibility.Visible;
            Answer_TextBox.LayoutUpdated += (sender, e) => MoveCustomCaret();
            FocusManager.SetFocusedElement(this, Answer_TextBox);
            Startup();
        }
        private void Startup()
        {
            string error, stats;
            historyIndex = 0;
            history.Clear();
            failure = false;
            quiz.Clear();
            target = 0;
            pos = 0;

            (quiz, conf, error, stats, failure, target) = files.GenerateFiles();
            if (stats == "null" && error == "null")
            {
                intersectImgDelay = (int)conf["Intersect Image Delay"];
                intersectMessage = (string)conf["Intersect Message"];
                welcomeMessage = (string)conf["Welcome Message"];
                intersectClipboard = (string)conf["Clipboard"];
                skip = (bool)conf["Skip"];
                conf = null;
                Quiz_Label.Content = quiz[pos].GetQuestion();
            }
            else
            {
                if (conf.ContainsKey("Debug")) { debug = (bool)conf["Debug"]; }
                Exception(error, stats);
            }
        }

        private void MoveCustomCaret()
        {
            Point caretLocation = Answer_TextBox.GetRectFromCharacterIndex(Answer_TextBox.CaretIndex).Location;

            if (!double.IsInfinity(caretLocation.X))
            {
                Canvas.SetLeft(Caret, caretLocation.X);
            }

            if (!double.IsInfinity(caretLocation.Y))
            {
                Canvas.SetTop(Caret, caretLocation.Y);
            }
        }

        private async void ErrorAnimation()
        {
            BorderRetangle.Stroke = darkRed;
            await Task.Delay(500);
            BorderRetangle.Stroke = white;
        }

        private static string ReplaceWhitespace(string input, string replacement)
        {
            Regex sWhitespace = new Regex(@"\s+");
            return sWhitespace.Replace(input, replacement);
        }

        private void Exception(string error, string stats)
        {
            Quiz_Label.Foreground = darkRed;
            Answer_TextBox.Foreground = darkRed;
            BorderRetangle.Stroke = darkRed;
            Quiz_Label.Content = stats;
            if (debug == true) { Clipboard.SetText(error); }
        }

        private void CalculateInput()
        {
            if (pos == 0 && quiz[quiz.Count - 1].GetAnswer().ToLower() == Answer_TextBox.Text.ToLower() && skip)
            {
                pos = quiz.Count - 1;
                UnlockIntersect();
            }
            else
            {
                if (Answer_TextBox.Text.ToLower() == quiz[pos].GetAnswer().ToLower())
                {
                    pos += 1;
                    if (pos == quiz.Count())
                    {
                        UnlockIntersect();
                    }
                    else
                    {
                        Quiz_Label.Content = quiz[pos].GetQuestion();
                        Answer_TextBox.Text = "";
                    }
                }
                else
                {
                    ErrorAnimation();
                    Answer_TextBox.Text = "";
                }
            }
        }
        private void UnlockIntersect()
        {
            Quiz_Label.Content = welcomeMessage;
            InputAnimation.Visibility = Visibility.Hidden;
            Answer_TextBox.Visibility = Visibility.Hidden;
            ConclusionLabel.Visibility = Visibility.Visible;
            BorderRetangle.Visibility = Visibility.Hidden;
            ConclusionLabel2.Visibility = Visibility.Visible;
            EnterBTN.Visibility = Visibility.Visible;
            pos = -1;
        }

        private async void Intersect()
        {
            ConclusionLabel2.Visibility = Visibility.Hidden;
            IntersectImages.Visibility = Visibility.Visible;
            ConclusionLabel.Visibility = Visibility.Hidden;
            Quiz_Label.Visibility = Visibility.Hidden;
            EnterBTN.Visibility = Visibility.Hidden;

            List<string> images = new List<string>();
            Stream mediaStream;
            int index = 0;

            images = Directory.GetFiles(Exe_Path + "/System Files/Intersect/", "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".gif") || s.EndsWith(".jpg") || s.EndsWith(".tiff")).ToList();
            foreach (string image in images)
            {
                BitmapImage bitmap = new BitmapImage();
                mediaStream = File.OpenRead(images[index]);

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = mediaStream;
                bitmap.EndInit();
                bitmap.Freeze();
                IntersectImages.Source = bitmap;
                await Task.Delay(intersectImgDelay);

                if (mediaStream != null)
                {
                    mediaStream.Close();
                    mediaStream.Dispose();
                    mediaStream = null;
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                }
                index += 1;
            }

            IntersectImages.Visibility = Visibility.Hidden;
            Quiz_Label.Content = intersectMessage;
            Quiz_Label.Visibility = Visibility.Visible;
            Clipboard.SetText(intersectClipboard);
            pos = -4;
        }

        private void AnswerTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        private void AnswerTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Answer_TextBox.Text = "";
                historyIndex = history.Count - 1;
            }
            else if (e.Key == Key.Up)
            {
                if (historyIndex > -1)
                {
                    historyIndex -= 1;
                }

                if (historyIndex >= 0 && historyIndex < history.Count)
                {
                    Answer_TextBox.Text = history[historyIndex];
                }
                else
                {
                    Answer_TextBox.Text = "";
                }
            }
            else if (e.Key == Key.Down)
            {
                if (historyIndex < history.Count)
                {
                    historyIndex += 1;
                }
                if (historyIndex >= 0 && historyIndex < history.Count)
                {
                    Answer_TextBox.Text = history[historyIndex];
                }
                else
                {
                    Answer_TextBox.Text = "";
                }
            }
            else if (e.Key == Key.Return)
            {
                if (failure && Answer_TextBox.Text.ToLower() == "ok")
                {
                    if (target == 1)
                    {
                        files.DeleteQuiz();
                    }
                    else if (target == 2)
                    {
                        files.DeleteConf();
                    }
                    else
                    {
                        files.DeleteQuiz();
                        files.DeleteConf();
                    }
                    failure = false;
                    Quiz_Label.Foreground = white;
                    BorderRetangle.Stroke = white;
                    Answer_TextBox.Foreground = white;
                    Answer_TextBox.Text = "";
                    Startup();
                }
                else if (failure)
                {
                    Answer_TextBox.Text = "";
                }
                else
                {
                    if (Answer_TextBox.Text != "" && ReplaceWhitespace(Answer_TextBox.Text, "") != "")
                    {
                        history.Add(Answer_TextBox.Text);
                    }
                    historyIndex = history.Count;
                    if (history.Count == 101) { history.RemoveRange(0, 1); }
                    CalculateInput();
                }
            }
        }

        private void EnterBTN_Click(object sender, RoutedEventArgs e)
        {
            Intersect();
        }

        private void IntersectWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && pos == -2)
            {
                EnterBTN.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6486f5"));
            }
        }

        private void IntersectWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (pos == -1)
            {
                pos = -2;
            }
            else if (e.Key == Key.Return && pos == -2)
            {
                pos = -3;
                EnterBTN.Background = new SolidColorBrush(Colors.Transparent);
                Intersect();
            }
            else if ((e.Key == Key.Return || e.Key == Key.Escape) && pos == -4)
            {
                IntersectWindow.Close();
            }
        }
    }
}