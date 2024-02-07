namespace Utils.Editor.GenericSearchWindow
{
    public class SearchEntry<T>
    {
        private string _path;
        public string Path => _path;

        private T _value;
        public T Value => _value;

        public SearchEntry(string path, T value)
        {
            _path = path;
            _value = value;
        }
    }
}