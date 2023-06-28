using UnityEngine;

namespace Utils.Attributes
{
    public class PathSelectorAttribute : PropertyAttribute
    {
        public bool IsRelative { get; }
        public bool IsDirectory { get; }

        public PathSelectorAttribute(bool isRelative = true, bool isDirectory = false)
        {
            IsRelative = isRelative;
            IsDirectory = isDirectory;
        }
    }
}