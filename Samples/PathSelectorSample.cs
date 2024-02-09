using UnityEditor;
using UnityEngine;
using Utils.Attributes;

namespace Samples
{
    public class PathSelectorSample : MonoBehaviour
    {
        [SerializeField, PathSelector] private string _path;
        [SerializeField, PathSelector(isDirectory: true)] private string _directory;
        [SerializeField, PathSelector(false, true)] private string _absoluteDirectory;
        [SerializeField, PathSelector(false, true)] private DefaultAsset _defaultAsset;
        [SerializeField, PathSelector(false, false)] private DefaultAsset _defaultAssetFile;
        [SerializeField] private string _normalString;
    }
}