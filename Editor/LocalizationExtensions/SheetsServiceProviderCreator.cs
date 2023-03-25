using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.Google;
using UnityEngine;

namespace Utils.Editor.LocalizationExtensions
{
    public static class SheetsServiceProviderCreator
    {
        [MenuItem("Assets/Localization extensions/Create sheets service provider")]
        public static void CreateProvider()
        {
            // check for stored credentials
            var success = TryGetCredentials(out var credentials);
            if (success)
            {
                var sheetServiceProvider = ScriptableObject.CreateInstance<SheetsServiceProvider>();
                sheetServiceProvider.SetOAuthCredentials(credentials.ClientId, credentials.ClientSecrets);
                var stringTable = Selection.activeObject as StringTableCollection;
                stringTable.Extensions
                    .Where(ext => ext is GoogleSheetsExtension)
                    .Cast<GoogleSheetsExtension>()
                    .ToList()
                    .ForEach(ext =>
                    {
                        ext.SheetsServiceProvider = sheetServiceProvider;
                    });
            }
        }

        [MenuItem("Assets/Localization extensions/Create sheets service provider", true)]
        public static bool CreateProviderValidation()
        {
            return Selection.activeObject is StringTableCollection;
        }
        
        [MenuItem("Tools/Facticus/Utils/Localization extensions/Remove sheets service provider stored credentials")]
        public static void RemoveCredentials()
        {
            EditorInputDialog.Show<ScriptableObject>(
                "Confirmation",
                "Are you sure you want to delete the Google OAuth credentials?",
                (_) =>
                {
                    var filePath = GetCredentialsPath();
                    File.Delete(filePath);
                },
                "Yes",
                "No",
                true
                );
        }
        
        private static bool TryGetCredentials(out CredentialsPersistentSettings credentials)
        {
            if (TryLoadCredentials(out credentials))
            {
                return true;
            }
            else
            {
                var entered = TryAskForCredentialsDialog(out credentials);
                return entered;
            }
        }
        
        private static bool TryAskForCredentialsDialog(out CredentialsPersistentSettings credentials)
        {
            credentials = EditorInputDialog.ShowModal<CredentialsPersistentSettings>(
                "Enter credentials", "Enter Google OAuth credentials");
            
            bool entered = credentials != null;

            if (entered)
            {
                SaveCredentials(credentials);
            }
            
            return entered;
        }
        
        private static bool TryLoadCredentials(out CredentialsPersistentSettings credentials)
        {
            try
            {
                var filePath = GetCredentialsPath();
                var file = File.Open(filePath, FileMode.Open);
        
                using var reader = new StreamReader(file);
                var json = reader.ReadToEnd();
                credentials = ScriptableObject.CreateInstance<CredentialsPersistentSettings>();
                JsonUtility.FromJsonOverwrite(json, credentials);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                credentials = null;
                return false;
            }
        }
        
        private static void SaveCredentials(CredentialsPersistentSettings credentials)
        {
            var filePath = GetCredentialsPath();
            var file = File.Open(filePath, FileMode.Create);

            using var writer = new StreamWriter(file);
            var json = JsonUtility.ToJson(credentials);
            writer.Write(json);
        }
        
        private static string GetCredentialsPath()
        {
            var tokenPath = Path.Combine(Application.persistentDataPath, "google_sheets_oauth_credentials.data");
            return tokenPath;
        }
    }
}