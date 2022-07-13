using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utils.Editor
{
    public class EditorFixSerializableReferences : EditorWindow
    {
        private string _description;

        private string _searchString;
        
        private string _oldAssembly;
        private string _oldNamespace;
        private string _oldClassName;
        private string _newAssembly;
        private string _newNamespace;
        private string _newClassName;
        private readonly (string, string)[,] _table = new (string, string)[3, 2];

        private bool _preview;
        private string _okButton;
        private string _cancelButton;
        private Action _onOkButton;
 
        private bool _initializedPosition;
        bool _shouldClose;

        private static bool IsValid(string str, out string error)
        {
            if (str != null && str.Trim().Contains(" "))
            {
                error = "can't contain white spaces";
                return false;
            }

            error = "";
            return true;
        }

        private static bool IsEmpty(string str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        }
        
        public static void FixSerializedReferences(string oldAssembly, string oldNamespace, string oldClassName,
            string newAssembly, string newNamespace, string newClassName, string searchString="", bool preview=false)
        {
            Assert.IsTrue(IsValid(oldAssembly, out var oldAssemblyError), $"oldAssembly {oldAssemblyError}");
            Assert.IsTrue(IsValid(oldNamespace, out var oldNamespaceError), $"oldNamespace {oldNamespaceError}");
            Assert.IsTrue(IsValid(oldClassName, out var oldClassNameError), $"oldClassName {oldClassNameError}");
            Assert.IsTrue(IsValid(newAssembly, out var newAssemblyError), $"newAssembly {newAssemblyError}");
            Assert.IsTrue(IsValid(newNamespace, out var newNamespaceError), $"newNamespace {newNamespaceError}");
            Assert.IsTrue(IsValid(newClassName, out var newClassNameError), $"newClassName {newClassNameError}");
            
            // replace with regex wildcard
            oldAssembly = IsEmpty(oldAssembly) ? "[^,}]*" : oldAssembly;
            oldNamespace = IsEmpty(oldNamespace) ? "[^,}]*" : oldNamespace;
            oldClassName = IsEmpty(oldClassName) ? "[^,}]*" : oldClassName;
            
            var guidsList = new List<string>();
            if (!string.IsNullOrEmpty(searchString))
            {
                var guids = AssetDatabase.FindAssets(searchString);
                guidsList.AddRange(guids);
            }
            else
            {
                guidsList.AddRange(AssetDatabase.FindAssets("t:ScriptableObject"));
                guidsList.AddRange(AssetDatabase.FindAssets("t:Scene"));
                guidsList.AddRange(AssetDatabase.FindAssets("t:Prefab"));
            }
            
            var assetsPaths = guidsList.ConvertAll(AssetDatabase.GUIDToAssetPath);
            var changes = new List<string>();

            foreach (var assetPath in assetsPaths)
            {
                var oldLines = File.ReadAllLines(assetPath);
                var newLines = new string[oldLines.Length];
                bool anyChange = false;
                for (int i = 0; i < oldLines.Length; i++)
                {
                    string line = oldLines[i];
                    string newLine = line;
                    var rx = new Regex(@"type: {class: (.*), ns: (.*), asm: (.*)}");
                    bool isReferenceLine = rx.IsMatch(line);
                    bool containsAssembly = ContainsPattern(line, "asm", oldAssembly);
                    bool containsNamespace = ContainsPattern(line, "ns", oldNamespace);
                    bool containsClassName = ContainsPattern(line, "class", oldClassName);
                    if (isReferenceLine && containsAssembly && containsNamespace && containsClassName)
                    {
                        if (!IsEmpty(newAssembly))
                        {
                            newLine = Replace(newLine, "asm", oldAssembly, newAssembly);
                            anyChange = true;
                        }
                        if (!IsEmpty(newNamespace))
                        {
                            newLine = Replace(newLine, "ns", oldNamespace, newNamespace);
                            anyChange = true;
                        }
                        if (!IsEmpty(newClassName))
                        {
                            newLine = Replace(newLine, "class", oldClassName, newClassName);
                            anyChange = true;
                        }

                        var changeString = $"-Line: {$"{i + 1}".PadRight(6)} File: {assetPath}" +
                                           $"\nold line: {line}" +
                                           $"\nnew line: {newLine}";
                        changes.Add(changeString);
                    }

                    newLines[i] = newLine;
                }

                if (anyChange && !preview)
                {
                    File.WriteAllLines(assetPath, newLines);
                }
            }
            
            if (changes.Count > 0)
                Debug.Log($"Changes in files:\n{string.Join("\n\n", changes)}");
        }

        private static string Replace(string input, string prefix, string oldPattern, string newValue)
        {
            var sectionRx = new Regex($"{prefix}: [^,}}]*");
            if (sectionRx.IsMatch(input))
            {
                var validSection = sectionRx.Matches(input)[0].Value;
                var rx = new Regex($"{prefix}: {oldPattern}");
                var newSection = rx.Replace(validSection, $"{prefix}: {newValue}");
                var replaced = input.Replace(validSection, newSection);
                return replaced;
            }

            return input;
        }

        private static bool ContainsPattern(string line, string prefix, string pattern)
        {
            var rx = new Regex($"{prefix}: [^,}}]*");
            if (rx.IsMatch(line))
            {
                var validSection = rx.Matches(line)[0].Value;
                bool containsPattern = new Regex($"{prefix}: {pattern}").IsMatch(validSection);
                return containsPattern;
            }

            return false;
        }

        [MenuItem("Tools/Facticus/Utils/Fix serialized references")]
        public static void FixSerializedReferences()
        {
            var window = CreateInstance<EditorFixSerializableReferences>();
            window.titleContent = new GUIContent("Fix serialized references");
            window._description = "Enter the old assembly, namespace and class name you want to replace";
            window._okButton = "Replace";
            window._cancelButton = "Cancel";
            window._onOkButton += () => FixSerializedReferences(
                window._table[0, 0].Item2,
                window._table[1, 0].Item2,
                window._table[2, 0].Item2,
                window._table[0, 1].Item2,
                window._table[1, 1].Item2,
                window._table[2, 1].Item2,
                window._searchString,
                window._preview
            );
            window.InitializeStringsTable();
            
            window.ShowModal();
        }

        private void InitializeStringsTable()
        {
            _table[0, 0] = ("Old assembly", "");
            _table[0, 1] = ("New assembly", "");
            _table[1, 0] = ("Old namespace", "");
            _table[1, 1] = ("New namespace", "");
            _table[2, 0] = ("Old class name", "");
            _table[2, 1] = ("New class name", "");
        }
        
        private void OnGUI()
        {
            // Check if Esc/Return have been pressed
            var e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    // Escape pressed
                    case KeyCode.Escape:
                        _shouldClose = true;
                        e.Use();
                        break;
 
                    // Enter pressed
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        _onOkButton?.Invoke();
                        _shouldClose = true;
                        e.Use();
                        break;
                }
            }
 
            if (_shouldClose) {  // Close this dialog
                Close();
                //return;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(_description);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("[Optional] Assets search string");
            _searchString = EditorGUILayout.TextField("", _searchString);

            // draw assemblies, namespaces and class names input fields
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.BeginVertical();
                
                EditorGUILayout.LabelField(_table[i, 0].Item1);
                _table[i, 0].Item2 = EditorGUILayout.TextField("", _table[i, 0].Item2);
                
                EditorGUILayout.LabelField(_table[i, 1].Item1);
                _table[i, 1].Item2 = EditorGUILayout.TextField("", _table[i, 1].Item2);
                
                EditorGUILayout.EndVertical();
            }
 
            EditorGUILayout.EndHorizontal();
            
            // Draw OK / Cancel buttons
            var r = EditorGUILayout.GetControlRect();

            float halfHorizontalSpacing = 4;
            
            r.width /= 3;
            r.width -= halfHorizontalSpacing;
            float horizontalOffset = r.width + 2 * halfHorizontalSpacing;

            _preview = GUI.Toggle(r, _preview, "preview");
            r.x += horizontalOffset;

            string replaceButtonName = _preview ? "Preview" : _okButton;

            if (GUI.Button(r, replaceButtonName)) {
                _onOkButton?.Invoke();
                if (!_preview)
                    _shouldClose = true;
            }

            r.x += horizontalOffset;
            if (GUI.Button(r, _cancelButton)) {
                _shouldClose = true;
            }
 
            EditorGUILayout.EndVertical();

            // restrict size
            var kLabelFloatMinW = (float) ((double) EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + 5.0);
            float width = kLabelFloatMinW * 3.2f;
            float height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 9f;
            minSize = new Vector2( width, height);
            maxSize = new Vector2(maxSize.x, height);

            // Set dialog position next to mouse position
            if (!_initializedPosition && e.type == EventType.Layout)
            {
                InitializePosition();
            }
        }

        private void InitializePosition()
        {
            _initializedPosition = true;

            // Move window to a new position. Make sure we're inside visible window
            var maxPos = GUIUtility.GUIToScreenPoint(new Vector2(Screen.width, Screen.height));
            var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            mousePos.x += 32;
            if (mousePos.x + position.width > maxPos.x) mousePos.x -= position.width + 64; // Display on left side of mouse
            if (mousePos.y + position.height > maxPos.y) mousePos.y = maxPos.y - position.height;

            position = new Rect(mousePos.x, mousePos.y, position.width, position.height);

            // Focus current window
            Focus();
        }

        private void ResizeToContent(Rect rect)
        {
            // Force change size of the window
            if (rect.width != 0 && minSize != rect.size)
            {
                minSize = maxSize = rect.size;
            }
        }
    }
}