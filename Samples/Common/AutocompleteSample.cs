using UnityEngine;
using Utils.Attributes;

namespace Samples.Common
{
    public class AutocompleteSample : MonoBehaviour
    {
        [SerializeField, Autocomplete] private string field1;
        [SerializeField, Autocomplete] private string field2;
        [SerializeField, Autocomplete] private string field3;
        // [SerializeField, Autocomplete("Test group")] private string test2;
        // [SerializeField, Autocomplete("Test group")] private string test3;
    }
}