using GK.WebScraping.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GK.WebScraping.App.Utilities
{
    public static class ConsoleAgent
    {
        private static RichTextBox _container;
        //private static SpeechSynthesizer _speechSynth;
        public static void Init(RichTextBox tb)
        {
            _container = tb;
        }

        //public static void UpdateConfig(Configurations config)
        //{
        //    //if (config.SupportAudioWarning)
        //    //{
        //    //    _speechSynth = new SpeechSynthesizer();
        //    //    _speechSynth.Volume = 50;
        //    //    _speechSynth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
        //    //}
        //    //else
        //    //{
        //    //    _speechSynth.Dispose();
        //    //    _speechSynth = null;
        //    //}

        //}

        public static void Write(string format, String color = "white", String bgcolor = "black", Boolean doSpeak = false, params Object[] args)
        {
            ConsoleAgent.Write(String.Format(format, args), color, bgcolor, doSpeak);
        }

        public static void Write(string v, String color = "white", String bgcolor = "black", Boolean doSpeak = false)
        {

            if (String.IsNullOrEmpty(v))
                return;

            Color foreColor = GetColor(color);
            //Color backColor = GetColor(color);
            if (ConsoleAgent._container.InvokeRequired)
            {
                ConsoleAgent._container.Invoke(new Action(() => WriteAsync(v, foreColor, doSpeak)));
            }
            else
            {
                _container.SelectionStart = _container.TextLength;
                _container.SelectionLength = v.Length;
                //_container.SelectionBackColor = backColor;
                _container.SelectionColor = foreColor;
                _container.AppendText(v + Environment.NewLine);
                _container.ScrollToCaret();
            }


            //if (_speechSynth != null && doSpeak)
            //    _speechSynth.SpeakAsync(v);
        }

        private static void WriteAsync(string v, Color foreColor, Boolean doSpeak)
        {
            _container.SelectionStart = _container.TextLength;
            _container.SelectionLength = v.Length;
            //_container.SelectionBackColor = backColor;
            _container.SelectionColor = foreColor;
            _container.AppendText(v + Environment.NewLine);
            _container.ScrollToCaret();


            //if (_speechSynth != null && doSpeak)
            //    _speechSynth.SpeakAsync(v);
        }

        //public static void Speak(String v)
        //{
        //    if (_speechSynth != null)
        //        _speechSynth.SpeakAsync(v);
        //}


        private static Color GetColor(string color)
        {
            Color c;
            switch (color.ToLower())
            {
                case "white": c = Color.White; break;
                case "red": c = Color.OrangeRed; break;
                case "green": c = Color.LightGreen; break;
                case "blue": c = Color.DeepSkyBlue; break;
                case "yellow": c = Color.Gold; break;
                case "black": c = Color.Black; break;
                case "gray": c = Color.Gray; break;
                case "purple": c = Color.MediumSlateBlue; break;
                default:
                    throw new NotImplementedException();
            }

            return c;
        }

        public static void Write(Exception ex)
        {
            ConsoleAgent.Write(format: "An error occured: {0}", color: "red", bgcolor: "gray", doSpeak: false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }

        public static void Clear()
        {
            _container.Text = String.Empty;

        }
    }
}
