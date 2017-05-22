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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TautologyDetector
{

    public class Word
    {
        public string original { get; }
        public string word { get; }
        public List<int> pairNum { get; set; }
        public bool selected { get; set; }
        public Word(string originalWord)
        {
            this.original = originalWord;
            var q = originalWord.ToCharArray().ToList();
            q.RemoveAll(o => !char.IsLetter(o));
            this.word = new string(q.ToArray()).ToLower();
            pairNum = new List<int>();

        }
    }

    public class WordsCouple
    {
        public Word one { get; set; }
        public Word two { get; set; }
        public int distance { get; set; }

        public float weight => (float)one.word.Length / distance;

        public override string ToString()
        {
            return one.word;
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();


        public MainWindow()
        {
            InitializeComponent();
            textBox.Document.Blocks.Clear();
            textBox.AppendText("Проверка тест проверка, ляляля тест проверка.");
            ProcessText();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // if (e.Key == Key.F1)

        }

        private List<Word> words;
        private void ProcessText()
        {
            string richText = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text;
            words = TextToWords(richText);
            List<WordsCouple> couples = new List<WordsCouple>();
            for (int i = 0; i < words.Count; i++)
                for (int j = i + 1; j < words.Count; j++)
                {
                    var word1 = words[i];
                    var word2 = words[j];
                    if (word1.word == word2.word)
                    {
                        couples.Add(new WordsCouple()
                        {
                            distance = j - i,
                            one = word1,
                            two = word2
                        });
                    }
                }
            var ordered = couples.OrderByDescending(o => o.weight).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {

                couples[i].one.pairNum.Add(i);
                couples[i].two.pairNum.Add(i);
            }
            WriteResult();

            listBox.Items.Clear();
            foreach (var couple in couples)
            {
                listBox.Items.Add(couple);
            }
        }


        private void WriteResult()
        {
            string contentString = "<meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\'> ";
            foreach (var now in words)
            {
                if (now.selected)
                {
                    contentString += "<span   style=\"background-color: #ff9900;\">" + now.original + "</span>";
                }
                else
                if (now.pairNum.Count > 0)
                    contentString += "<span style=\"color: green;\">" + now.original + "</span>";
                else
                {
                    contentString += now.original;
                }
               
                

                contentString += ' ';
            }
            webbrowser.NavigateToString(contentString);
        }
        private List<Word> TextToWords(string text)
        {
            List<Word> words = new List<Word>();
            foreach (var s in text.Split(' '))
            {
                words.Add(new Word(s));
            }
            return words;
        }

        private Color GetRandomColor()
        {
            return Color.FromArgb(255, (byte)rnd.Next(0, 256), (byte)rnd.Next(256), (byte)rnd.Next(256));
        }

        private void textBox_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var now in words)
            {
                now.selected = false;
            }
            var couple = listBox.SelectedItems[0] as WordsCouple;
            if (couple != null)
                couple.one.selected = true;
            var wordsCouple = listBox.SelectedItems[0] as WordsCouple;
            if (wordsCouple != null)
                wordsCouple.two.selected = true;
            WriteResult();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ProcessText();
        }
    }
}
