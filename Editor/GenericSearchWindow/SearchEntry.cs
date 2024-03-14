using UnityEngine;

namespace Utils.Editor.GenericSearchWindow
{
    public class SearchEntry<T>
    {
        private string _path;
        public string Path => _path;

        private T _value;
        public T Value => _value;

        private Texture _icon;
        public Texture Icon => _icon;

        public SearchEntry(string path, T value)
        {
            _path = path;
            _value = value;
        }
        
        public SearchEntry(string path, T value, Texture icon)
        {
            _path = path;
            _value = value;
            _icon = icon;
        }
    }
}