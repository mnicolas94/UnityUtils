using System.Linq;
using UnityEngine;
using Utils;

namespace Samples.Common
{
    public class MathUtilsTests : MonoBehaviour
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;

        [ContextMenu("Print split")]
        public void PrintSplit()
        {
            var numbers = MathUtils.SplitNicely(_min, _max);
            var numbersStrings = numbers.Select(n => n.ToString()).ToList();
            string s = string.Join(" ", numbersStrings);
            Debug.Log(s);
        }
    }
}