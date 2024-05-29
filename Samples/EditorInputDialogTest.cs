#if UNITY_EDITOR
using UnityEngine;
using Utils.Editor;

namespace Samples
{
    public class EditorInputDialogTest : MonoBehaviour
    {
        [SerializeField] private string _message;
        
        [ContextMenu("Show message")]
        public void ShowMessage()
        {
            EditorInputDialog.ShowMessage("Message", _message);
        }
    }
}
#endif