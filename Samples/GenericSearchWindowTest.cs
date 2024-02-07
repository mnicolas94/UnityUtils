using System.Collections.Generic;
using UnityEngine;
using Utils.Editor.GenericSearchWindow;

namespace Samples
{
    public class GenericSearchWindowTest : MonoBehaviour
    {
        [ContextMenu(nameof(Test))]
        public void Test()
        {
            var entries = new List<SearchEntry<string>>
            {
                new ("group1/group1-subgroup1/entry1", "group1/group1-subgroup1/entry1"),
                new ("group1/group1-subgroup1/entry2", "group1/group1-subgroup1/entry2"),
                new ("group1/group1-subgroup1/entry3", "group1/group1-subgroup1/entry3"),
                new ("group1/group1-subgroup2/entry1", "group1/group1-subgroup2/entry1"),
                new ("group1/group1-subgroup2/entry2", "group1/group1-subgroup2/entry2"),
                new ("group1/group1-subgroup3/entry1", "group1/group1-subgroup3/entry1"),
                new ("group3/entry1", "group3/entry1"),
                new ("group2/entry1", "group2/entry1"),
                new ("group2/entry2", "group2/entry2"),
                new ("entry1", "entry1"),
            };
            GenericSearchWindowProvider<string>.Create(Vector2.zero, "Title strings", entries, Debug.Log);
        }
    }
}