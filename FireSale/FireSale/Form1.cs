using System;
using System.Speech.Recognition;
using System.Threading;
using System.Windows.Forms;

namespace FireSale
{
    public partial class Form1 : Form
    {
        public SpeechRecognitionEngine recognizer;
             
        public Grammar grammar;

        public Thread RecThread;
        public bool RecognizerState = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Here we first need to setup the grammar rules:
            GrammarBuilder build = new GrammarBuilder();
            build.AppendDictation();
            grammar = new Grammar(build);
            //In here we initialize the recognizer and setup its events:
            recognizer = new SpeechRecognitionEngine();      
            recognizer.LoadGrammarAsync(grammar);
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Recognizer_SpeechRecognized);
            //Here will initialize the recognizer thread:
            RecognizerState = true;
            RecThread = new Thread(new ThreadStart(RecThreadFunction));
            RecThread.Start();

        }

        public void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            //Recognizer recognizes the speech

            if (!RecognizerState)
                return;

            Invoke((MethodInvoker)delegate
            {
                textBox1.Text += (" " + e.Result.Text.ToLower());
                //This will add a space between each word you say
            });

           
        }
        public void RecThreadFunction()
        {
            //This is on separate threads. Will loop the recognizer when receiving calls

            while (true)
            {
                try
                {
                    recognizer.Recognize();
                }
                catch
                {
                    //Handles errors
                    //Won't hear you, nothing will happen
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RecognizerState = true;
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RecognizerState = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RecThread.Abort();
            RecThread = null;

            recognizer.UnloadAllGrammars();
            recognizer.Dispose();

            grammar = null;           
        }
    }
}
