using UnityEngine;

namespace Utils.Editor.LocalizationExtensions
{
    public class CredentialsPersistentSettings : ScriptableObject
    {
        [SerializeField] private string _clientId;
        [SerializeField] private string _clientSecrets;

        public string ClientId
        {
            get => _clientId;
            set => _clientId = value;
        }

        public string ClientSecrets
        {
            get => _clientSecrets;
            set => _clientSecrets = value;
        }
    }
}