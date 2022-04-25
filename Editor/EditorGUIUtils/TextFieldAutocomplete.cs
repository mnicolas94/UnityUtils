using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor.EditorGUIUtils
{
    /// <summary>
    /// Code from https://www.clonefactor.com/wordpress/program/c/1809/
    /// </summary>
    public static class TextFieldAutocomplete
    {
        #region Text AutoComplete
        /// <summary>The internal struct used for AutoComplete (Editor)</summary>
        private struct EditorAutoCompleteParams
        {
            public const string FieldTag = "AutoCompleteField";
        public static readonly Color FancyColor = new Color(.6f, .6f, .7f);
        public static readonly float optionHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            public const int fuzzyMatchBias = 3; // input length smaller then this letter, will not trigger fuzzy checking.
            public static List<string> CacheCheckList = null;
            public static string lastInput;
            public static string focusTag = "";
            public static string lastTag = ""; // Never null, optimize for length check.
            public static int selectedOption = -1; // record current selected option.
            public static Vector2 mouseDown;
            public static void CleanUpAndBlur()
            {
                selectedOption = -1;
                GUI.FocusControl("");
            }
        }
        /// <summary>A textField to popup a matching popup, based on developers input values.</summary>
        /// <param name="input">string input.</param>
        /// <param name="source">the data of all possible values (string).</param>
        /// <param name="maxShownCount">the amount to display result.</param>
        /// <param name="levenshteinDistance">
        /// value between 0f ~ 1f, (percent)
        /// - more then 0f will enable the fuzzy matching
        /// - 1f = 100% error threshold = anything thing is okay.
        /// - 0f = 000% error threshold = require full match to the reference
        /// - recommend 0.4f ~ 0.7f
        /// </param>
        /// <returns>output string.</returns>
        public static string TextFieldAutoComplete(string input, string[] source, int maxShownCount = 5, float levenshteinDistance = 0.5f)
        {
            return TextFieldAutoComplete(EditorGUILayout.GetControlRect(), input, source, maxShownCount, levenshteinDistance);
        }
        /// <summary>A textField to popup a matching popup, based on developers input values.</summary>
        /// <param name="position">EditorGUI position</param>
        /// <param name="input">string input.</param>
        /// <param name="source">the data of all possible values (string).</param>
        /// <param name="maxShownCount">the amount to display result.</param>
        /// <param name="levenshteinDistance">
        /// value between 0f ~ 1f, (percent)
        /// - more then 0f will enable the fuzzy matching
        /// - 1f = 100% error threshold = everything is okay.
        /// - 0f = 000% error threshold = require full match to the reference
        /// - recommend 0.4f ~ 0.7f
        /// </param>
        /// <returns>output string.</returns>
        public static string TextFieldAutoComplete(Rect position, string input, string[] source, int maxShownCount = 5, float levenshteinDistance = 0.5f)
        {
            // Text field
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            string tag = EditorAutoCompleteParams.FieldTag + controlId;
            GUI.SetNextControlName(tag);
            string rst = EditorGUI.TextField(position, input, EditorStyles.popup);
            // Matching with giving source
            if (input.Length > -1 && // have input
                (EditorAutoCompleteParams.lastTag.Length == 0 || EditorAutoCompleteParams.lastTag == tag) && // one frame delay for process click event.
                GUI.GetNameOfFocusedControl() == tag) // focus this control
            {
                // Matching
                if (EditorAutoCompleteParams.lastInput != input || // input changed
                    EditorAutoCompleteParams.focusTag != tag) // switch focus from another field.
                {
                    // Update cache
                    EditorAutoCompleteParams.focusTag = tag;
                    EditorAutoCompleteParams.lastInput = input;
                    List<string> uniqueSrc = new List<string>(new HashSet<string>(source)); // remove duplicate
                    int srcCnt = uniqueSrc.Count;
                    EditorAutoCompleteParams.CacheCheckList = new List<string>(System.Math.Min(maxShownCount, srcCnt)); // optimize memory alloc
                    // Start with - slow
                    for (int i = 0; i < srcCnt && EditorAutoCompleteParams.CacheCheckList.Count < maxShownCount; i++)
                    {
                        if (uniqueSrc[i].ToLower().StartsWith(input.ToLower()))
                        {
                            EditorAutoCompleteParams.CacheCheckList.Add(uniqueSrc[i]);
                            uniqueSrc.RemoveAt(i);
                            srcCnt--;
                            i--;
                        }
                    }
                    // Contains - very slow
                    if (EditorAutoCompleteParams.CacheCheckList.Count == 0)
                    {
                        for (int i = 0; i < srcCnt && EditorAutoCompleteParams.CacheCheckList.Count < maxShownCount; i++)
                        {
                            if (uniqueSrc[i].ToLower().Contains(input.ToLower()))
                            {
                                EditorAutoCompleteParams.CacheCheckList.Add(uniqueSrc[i]);
                                uniqueSrc.RemoveAt(i);
                                srcCnt--;
                                i--;
                            }
                        }
                    }
                    // Levenshtein Distance - very very slow.
                    if (levenshteinDistance > 0f && // only developer request
                        input.Length > EditorAutoCompleteParams.fuzzyMatchBias && // bias on input, hidden value to avoid doing it too early.
                        EditorAutoCompleteParams.CacheCheckList.Count < maxShownCount) // have some empty space for matching.
                    {
                        levenshteinDistance = Mathf.Clamp01(levenshteinDistance);
                        string keywords = input.ToLower();
                        for (int i = 0; i < srcCnt && EditorAutoCompleteParams.CacheCheckList.Count < maxShownCount; i++)
                        {
                            int distance = StringExtend.LevenshteinDistance(uniqueSrc[i], keywords, caseSensitive: false);
                            bool closeEnough = (int)(levenshteinDistance * uniqueSrc[i].Length) > distance;
                            if (closeEnough)
                            {
                                EditorAutoCompleteParams.CacheCheckList.Add(uniqueSrc[i]);
                                uniqueSrc.RemoveAt(i);
                                srcCnt--;
                                i--;
                            }
                        }
                    }
                }
                // Draw recommend keyward(s)
                if (EditorAutoCompleteParams.CacheCheckList.Count > 0)
                {
                    Event evt = Event.current;
                    int cnt = EditorAutoCompleteParams.CacheCheckList.Count;
                    float height = cnt * EditorAutoCompleteParams.optionHeight;
                    Rect area = new Rect(position.x, position.y - height, position.width, height);
                
                    // Fancy color UI
//                    EditorGUI.BeginDisabledGroup(true);
//                    EditorGUI.DrawRect(area, EditorAutoCompleteParams.FancyColor);
                    var style = new GUIStyle(GUI.skin.button);
                    GUI.Label(area, GUIContent.none, style);
//                    EditorGUI.EndDisabledGroup();
                    // Click event hack - part 1
                    // cached data for click event hack.
                    if (evt.type == EventType.Repaint)
                    {
                        // Draw option(s), if we have one.
                        // in repaint cycle, we only handle display.
                        Rect line = new Rect(area.x, area.y, area.width, EditorAutoCompleteParams.optionHeight);
                        for (int i = 0; i < cnt; i++)
                        {
//                        EditorGUI.ToggleLeft(line, GUIContent.none, (input == EditorAutoCompleteParams.CacheCheckList[i]));
                            Rect option = EditorGUI.IndentedRect(line);
                            if (line.Contains(evt.mousePosition))
                            {
                                // hover style
                                EditorGUI.LabelField(option, EditorAutoCompleteParams.CacheCheckList[i], GUI.skin.textArea);
                                EditorAutoCompleteParams.selectedOption = i;
                                GUIUtility.hotControl = controlId; // required for Cursor skin. (AddCursorRect)
                                EditorGUIUtility.AddCursorRect(area, MouseCursor.ArrowPlus);
                            }
                            else if (EditorAutoCompleteParams.selectedOption == i)
                            {
                                // hover style
                                EditorGUI.LabelField(option, EditorAutoCompleteParams.CacheCheckList[i], GUI.skin.textArea);
                            }
                            else
                            {
                                EditorGUI.LabelField(option, EditorAutoCompleteParams.CacheCheckList[i], EditorStyles.label);
                            }
                            line.y += line.height;
                        }
                        // when hover popup, record this as the last usein tag.
                        if (area.Contains(evt.mousePosition) && EditorAutoCompleteParams.lastTag != tag)
                        {
                            // Debug.Log("->" + tag + " Enter popup: " + area);
                            // used to trigger the clicked checking later.
                            EditorAutoCompleteParams.lastTag = tag;
                        }
                    }
                    else if (evt.type == EventType.MouseDown)
                    {
                        if (area.Contains(evt.mousePosition) || position.Contains(evt.mousePosition))
                        {
                            EditorAutoCompleteParams.mouseDown = evt.mousePosition;
                        }
                        else
                        {
                            // click outside popup area, deselected - blur.
                            EditorAutoCompleteParams.CleanUpAndBlur();
                        }
                    }
                    else if (evt.type == EventType.MouseUp)
                    {
                        if (position.Contains(evt.mousePosition))
                        {
                            // common case click on textfield.
                            return rst;
                        }
                        else if (area.Contains(evt.mousePosition))
                        {
                            if (Vector2.Distance(EditorAutoCompleteParams.mouseDown, evt.mousePosition) >= 3f)
                            {
                                // Debug.Log("Click and drag out the area.");
                                return rst;
                            }
                            else
                            {
                                // Click event hack - part 3
                                // for some reason, this session only run when popup display on inspector empty space.
                                // when any selectable field behind of the popup list, Unity3D can't reaching this session.
                                _AutoCompleteClickhandle(position, ref rst);
                                EditorAutoCompleteParams.focusTag = string.Empty; // Clean up
                                EditorAutoCompleteParams.lastTag = string.Empty; // Clean up
                            }
                        }
                        else
                        {
                            // click outside popup area, deselected - blur.
                            EditorAutoCompleteParams.CleanUpAndBlur();
                        }
                        return rst;
                    }
                    else if (evt.isKey && evt.type == EventType.KeyUp)
                    {
                        switch (evt.keyCode)
                        {
                            case KeyCode.PageUp:
                            case KeyCode.UpArrow:
                                EditorAutoCompleteParams.selectedOption--;
                                if (EditorAutoCompleteParams.selectedOption < 0)
                                    EditorAutoCompleteParams.selectedOption = EditorAutoCompleteParams.CacheCheckList.Count - 1;
                                break;
                            case KeyCode.PageDown:
                            case KeyCode.DownArrow:
                                EditorAutoCompleteParams.selectedOption++;
                                if (EditorAutoCompleteParams.selectedOption >= EditorAutoCompleteParams.CacheCheckList.Count)
                                    EditorAutoCompleteParams.selectedOption = 0;
                                break;
                            case KeyCode.KeypadEnter:
                            case KeyCode.Return:
                                if (EditorAutoCompleteParams.selectedOption != -1)
                                {
                                    _AutoCompleteClickhandle(position, ref rst);
                                    EditorAutoCompleteParams.focusTag = string.Empty; // Clean up
                                    EditorAutoCompleteParams.lastTag = string.Empty; // Clean up
                                }
                                else
                                {
                                    EditorAutoCompleteParams.CleanUpAndBlur();
                                }
                                break;
                            case KeyCode.Escape:
                                EditorAutoCompleteParams.CleanUpAndBlur();
                                break;
                            default:
                                // hit any other key(s), assume typing, avoid override by Enter;
                                EditorAutoCompleteParams.selectedOption = -1;
                                break;
                        }
                    }
                }
            }
            else if (EditorAutoCompleteParams.lastTag == tag &&
                     GUI.GetNameOfFocusedControl() != tag)
            {
                // Click event hack - part 2
                // catching mouse click on blur
                _AutoCompleteClickhandle(position, ref rst);
                EditorAutoCompleteParams.lastTag = string.Empty; // reset
            }
            return rst;
        }
        /// <summary>calculate auto complete select option location, and select it.
        /// within area, and we display option in "Vertical" style.
        /// which line is what we care.
        /// </summary>
        /// <param name="rst">input string, may overrided</param>
        /// <param name="cnt"></param>
        /// <param name="area"></param>
        /// <param name="mouseY"></param>
        private static void _AutoCompleteClickhandle(Rect position, ref string rst)
        {
            int index = EditorAutoCompleteParams.selectedOption;
            Vector2 pos = EditorAutoCompleteParams.mouseDown; // hack: assume mouse are stay in click position (1 frame behind).
            if (0 <= index && index < EditorAutoCompleteParams.CacheCheckList.Count)
            {
                rst = EditorAutoCompleteParams.CacheCheckList[index];
                GUI.changed = true;
                // Debug.Log("Selecting index (" + EditorAutoCompleteParams.selectedOption + ") "+ rst);
            }
            else
            {
                // Fail safe, when selectedOption failure
            
                int cnt = EditorAutoCompleteParams.CacheCheckList.Count;
                float height = cnt * EditorAutoCompleteParams.optionHeight;
                Rect area = new Rect(position.x, position.y - height, position.width, height);
                if (!area.Contains(pos))
                    return; // return early.
                float lineY = area.y;
                for (int i = 0; i < cnt; i++)
                {
                    if (lineY < pos.y && pos.y < lineY + EditorAutoCompleteParams.optionHeight)
                    {
                        rst = EditorAutoCompleteParams.CacheCheckList[i];
                        Debug.LogError("Fail to select on \"" + EditorAutoCompleteParams.lastTag + "\" selected = " + rst + "\ncalculate by mouse position.");
                        GUI.changed = true;
                        break;
                    }
                    lineY += EditorAutoCompleteParams.optionHeight;
                }
            }
            EditorAutoCompleteParams.CleanUpAndBlur();
        }
        #endregion
    }

    public static class StringExtend
    {
        /// <summary>Computes the Levenshtein Edit Distance between two enumerables.</summary>
        /// <param name="lhs">The first enumerable.</param>
        /// <param name="rhs">The second enumerable.</param>
        /// <returns>The edit distance.</returns>
        /// <see cref="https://en.wikipedia.org/wiki/Levenshtein_distance"/>
        public static int LevenshteinDistance(string lhs, string rhs, bool caseSensitive = true)
        {
            if (!caseSensitive)
            {
                lhs = lhs.ToLower();
                rhs = rhs.ToLower();
            }
            char[] first = lhs.ToCharArray();
            char[] second = rhs.ToCharArray();
            return LevenshteinDistance<char>(first, second);
        }
    
        /// <summary>Computes the Levenshtein Edit Distance between two enumerables.</summary>
        /// <typeparam name="T">The type of the items in the enumerables.</typeparam>
        /// <param name="lhs">The first enumerable.</param>
        /// <param name="rhs">The second enumerable.</param>
        /// <returns>The edit distance.</returns>
        /// <see cref="https://blogs.msdn.microsoft.com/toub/2006/05/05/generic-levenshtein-edit-distance-with-c/"/>
        public static int LevenshteinDistance<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) where T : System.IEquatable<T>
        {
            // Validate parameters
            if (lhs == null) throw new System.ArgumentNullException("lhs");
            if (rhs == null) throw new System.ArgumentNullException("rhs");
            // Convert the parameters into IList instances
            // in order to obtain indexing capabilities
            IList<T> first = lhs as IList<T> ?? new List<T>(lhs);
            IList<T> second = rhs as IList<T> ?? new List<T>(rhs);
            // Get the length of both.  If either is 0, return
            // the length of the other, since that number of insertions
            // would be required.
            int n = first.Count, m = second.Count;
            if (n == 0) return m;
            if (m == 0) return n;
            // Rather than maintain an entire matrix (which would require O(n*m) space),
            // just store the current row and the next row, each of which has a length m+1,
            // so just O(m) space. Initialize the current row.
            int curRow = 0, nextRow = 1;
            int[][] rows = new int[][] { new int[m + 1], new int[m + 1] };
            for (int j = 0; j <= m; ++j)
                rows[curRow][j] = j;
            // For each virtual row (since we only have physical storage for two)
            for (int i = 1; i <= n; ++i)
            {
                // Fill in the values in the row
                rows[nextRow][0] = i;
                for (int j = 1; j <= m; ++j)
                {
                    int dist1 = rows[curRow][j] + 1;
                    int dist2 = rows[nextRow][j - 1] + 1;
                    int dist3 = rows[curRow][j - 1] +
                                (first[i - 1].Equals(second[j - 1]) ? 0 : 1);
                    rows[nextRow][j] = System.Math.Min(dist1, System.Math.Min(dist2, dist3));
                }
                // Swap the current and next rows
                if (curRow == 0)
                {
                    curRow = 1;
                    nextRow = 0;
                }
                else
                {
                    curRow = 0;
                    nextRow = 1;
                }
            }
            // Return the computed edit distance
            return rows[curRow][m];
        }
    }
}