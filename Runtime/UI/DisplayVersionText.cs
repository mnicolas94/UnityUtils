using TMPro;
using UnityEngine;

namespace Utils.UI
{
    public class DisplayVersionText : MonoBehaviour
    {
        [Multiline(4)]
        private string _versionFormatString = "game v{v}";
        [SerializeField] private TMP_Text _text;

        void Start()
        {
            _text.text = GetFormattedString(_versionFormatString);
        }
        
        public string GetFormattedString(string format)
        {
            var version = Application.version;
            var outString = format.Replace("{v}", version);
            outString = outString.Replace("{v_}", version.Replace('.', '_'));
            // outString = outString.Replace("{gitHash}", Version.Instance.GitHash);
            // outString = outString.Replace("{time}", Version.Instance.BuildTimestamp);

            return outString;
        }
    }
}