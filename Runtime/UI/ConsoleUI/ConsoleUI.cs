using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Runtime.UI.ConsoleUI
{
    public class ConsoleUI : MonoBehaviour
    {
        public TextMeshProUGUI textArea;
        public ScrollRect scroll;

        private StringBuilder _sb;

        private void Awake()
        {
            _sb = new StringBuilder();
        }

        void Start()
        {
            Clear();
        }

        public void LogInfo(string line)
        {
            WriteLineInConsole(line, Color.white);
        }
    
        public void LogWarning(string line)
        {
            WriteLineInConsole(line, Color.yellow);
        }

        public void LogError(string line)
        {
            WriteLineInConsole(line, Color.red);
        }

        public void Clear()
        {
            _sb.Clear();
            textArea.text = _sb.ToString();
        }

        private void WriteLineInConsole(string line, Color color)
        {
            int r = (int) (color.r * 255);
            int g = (int) (color.g * 255);
            int b = (int) (color.b * 255);
            _sb.Append("<color=#");
            _sb.Append(r.ToString("X2"));
            _sb.Append(g.ToString("X2"));
            _sb.Append(b.ToString("X2"));
            _sb.Append(">");
            _sb.Append(TimeString());
            _sb.Append(" ");
            _sb.Append(line);
            _sb.AppendLine("</color>");

            textArea.text = _sb.ToString();
            StartCoroutine(SeekBottom());
        }

        private string TimeString()
        {
            return DateTime.Now.ToString("hh:mm:ss");
        }

        private IEnumerator SeekBottom()
        {
            yield return new WaitForEndOfFrame();
            scroll.verticalNormalizedPosition = 0;
        }
    }
}
