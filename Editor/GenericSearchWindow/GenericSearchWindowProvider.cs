using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Utils.Editor.GenericSearchWindow
{
    public class GenericSearchWindowProviderScriptableObject : ScriptableObject, ISearchWindowProvider
    {
        [SerializeReference] private ISearchWindowProvider _providerDelegate;

        public void Initialize(ISearchWindowProvider providerDelegate)
        {
            _providerDelegate = providerDelegate;
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return _providerDelegate.CreateSearchTree(context);
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            return _providerDelegate.OnSelectEntry(SearchTreeEntry, context);
        }
    }
    
    public class GenericSearchWindowProvider<T> : ISearchWindowProvider
    {
        private string _title;
        private readonly List<SearchEntry<T>> _entries;
        private readonly Action<T> _onSelected;
        private readonly Texture2D _indentationIcon;

        public static void Create(Vector2 position, string title, List<SearchEntry<T>> entries, Action<T> onSelected)
        {
            var providerScriptableObject = ScriptableObject.CreateInstance<GenericSearchWindowProviderScriptableObject>();
            var genericProvider = new GenericSearchWindowProvider<T>(title, entries, onSelected);
            providerScriptableObject.Initialize(genericProvider);
            
            var searchWindowContext = new SearchWindowContext(position);
            SearchWindow.Open(searchWindowContext, providerScriptableObject);
        }
        
        private GenericSearchWindowProvider(string title, List<SearchEntry<T>> entries, Action<T> onSelected)
        {
            _title = title;
            _entries = entries;
            _entries.Sort((entryA, entryB) => string.Compare(entryA.Path, entryB.Path, StringComparison.Ordinal));
            _onSelected = onSelected;
            
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, Color.clear);
            _indentationIcon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            
            var title = new SearchTreeGroupEntry(new GUIContent(_title), 0);
            tree.Add(title);

            var groups = new List<string>();
            foreach (var searchEntry in _entries)
            {
                var path = searchEntry.Path;
                var entryGroups = path.Split('/');
                var groupName = "";
                for (int i = 0; i < entryGroups.Length - 1; i++)
                {
                    groupName += entryGroups[i];
                    if (!groups.Contains(groupName))
                    {
                        var entryGroup = new SearchTreeGroupEntry(new GUIContent(entryGroups[i]), i + 1);
                        tree.Add(entryGroup);
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }

                var entry = GetIndentedEntry(entryGroups.Last());
                entry.level = entryGroups.Length;
                entry.userData = searchEntry;
                tree.Add(entry);
            }
            
            return tree;
        }
        
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is SearchEntry<T> entry)
            {
                _onSelected?.Invoke(entry.Value);
                return true;
            }
            
            return false;
        }

        private SearchTreeEntry GetIndentedEntry(string entryText)
        {
            var entry = new SearchTreeEntry(new GUIContent(entryText, _indentationIcon));
            return entry;
        }
    }
}