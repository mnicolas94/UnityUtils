using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public static class SubAssetsUtils
    {
        private static List<Object> _pasteBin = new List<Object>();

        [MenuItem("Assets/Facticus/Utils/SubAssets management/Copy asset &c")]
        public static void CutAssetObject()
        {
            var selected = GetSelectedAssets();
            foreach (var obj in selected)
            {
                bool isFolder = AssetIsFolder(obj);
                if (!isFolder)
                {
                    _pasteBin.Add(obj);
                }
            }
        }

        [MenuItem("Assets/Facticus/Utils/SubAssets management/Paste into asset &v")]
        public static void PasteIntoAsset()
        {
            var selection = Selection.GetFiltered<Object>(SelectionMode.Assets);
            var selected = selection.Length > 0 ? selection[0] : null;
            bool isFolder = selected == null || AssetIsFolder(selected);
            if (isFolder)
            {
                foreach (var asset in _pasteBin)
                {
                    PasteFromMainAssetToFolder(asset, selected);
                }
            }
            else
            {
                foreach (var asset in _pasteBin)
                {
                    PasteIntoAsset(asset, selected);
                }
            }
            _pasteBin.Clear();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Facticus/Utils/SubAssets management/Delete sub asset")]
        public static void DeleteSubAsset()
        {
            var asset = Selection.activeObject;
            bool isSub = AssetDatabase.IsSubAsset(asset);
            if (isSub)
            {
                AssetDatabase.RemoveObjectFromAsset(asset);
            }
            
            AssetDatabase.SaveAssets();
        }
        
        [MenuItem("Assets/Facticus/Utils/SubAssets management/Delete all sub assets")]
        public static void ClearSubAssets()
        {
            var asset = Selection.activeObject;
            bool isMain = AssetDatabase.IsMainAsset(asset);
            if (isMain)
            {
                var subassets = GetSubAssets(asset);
                foreach (var obj in subassets)
                {
                    if (obj == null)
                    {
                        Object.DestroyImmediate(obj);
                    }
                    else
                    {
                        AssetDatabase.RemoveObjectFromAsset(obj);
                    }
                }
            
                // make change to take effect
                AssetDatabase.SaveAssets();
            }
        }
        
        private static void AddObjectToContainer(Object obj, Object container)
        {
            AssetDatabase.AddObjectToAsset(obj, container);
            EditorUtility.SetDirty(container);
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }

        private static List<Object> GetSelectedAssets()
        {
            var folders = Selection.GetFiltered<Object>(SelectionMode.Assets).Where(AssetIsFolder);
            var assets = Selection.GetFiltered<Object>(SelectionMode.Deep).Where(IsAsset);
            return folders.Concat(assets).ToList();
        }

        private static bool IsAsset(Object obj)
        {
            return AssetDatabase.IsMainAsset(obj) || AssetDatabase.IsSubAsset(obj);
        }

        private static void PasteIntoAsset(Object assetToCopy, Object parent)
        {
            string path = AssetDatabase.GetAssetPath(assetToCopy);
            bool isMain = AssetDatabase.IsMainAsset(assetToCopy);
            if (isMain)
            {
                var subAssets = GetSubAssets(assetToCopy);
                foreach (var subAsset in subAssets)
                {
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    AddObjectToContainer(subAsset, parent);
                }
                        
                AssetDatabase.RemoveObjectFromAsset(assetToCopy);
                AssetDatabase.DeleteAsset(path);
            }
            else
            {
                AssetDatabase.RemoveObjectFromAsset(assetToCopy);
            }

            AddObjectToContainer(assetToCopy, parent);
        }
        
        private static void PasteFromMainAssetToFolder(Object assetToPaste, Object folderAsset)
        {
            bool isSub = AssetDatabase.IsSubAsset(assetToPaste);
            if (isSub)
            {
                string folderPath;
                if (folderAsset == null)  // no folder is selected
                {
                    var success = TryGetActiveFolderPath(out var projectWindowFolder);
                    if (success)
                    {
                        folderPath = projectWindowFolder;
                    }
                    else
                    {
                        var assetPath = AssetDatabase.GetAssetPath(assetToPaste);
                        folderPath = Path.GetDirectoryName(assetPath);
                    }
                }
                else
                {
                    folderPath = AssetDatabase.GetAssetPath(folderAsset);
                }
                string path = Path.Combine(folderPath, $"{assetToPaste.name}.asset");
                AssetDatabase.RemoveObjectFromAsset(assetToPaste);
                AssetDatabase.CreateAsset(assetToPaste, path);
            }
        }
        
        private static bool TryGetActiveFolderPath(out string path)
        {
            var method = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic );
            object[] args = { null };
            bool found = (bool) method.Invoke( null, args );
            path = (string)args[0];
            return found;
        }
        
        public static bool AssetIsFolder(Object asset)
        {
            if (asset != null)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                if (path.Length > 0)
                {
                    bool isFolder = Directory.Exists(path);
                    return isFolder;
                }
                
                return false;
            }

            return false;
        }

        public static Object[] GetSubAssets(Object asset)
        {
            bool isMain = AssetDatabase.IsMainAsset(asset);
            if (isMain)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                var subobjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                return subobjects;
            }
            else
            {
                return new Object[]{};
            }
        }
    }
}