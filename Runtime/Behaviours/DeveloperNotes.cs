using UnityEngine;

namespace Utils.Behaviours
{
    public class DeveloperNotes : MonoBehaviour
    {
        [SerializeField, TextArea()] private string _notes;
    }
}