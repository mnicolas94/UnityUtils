using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils.UI
{
    public class RenamableText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Button _editButton;

        [SerializeField] private UnityEvent<string> _onEdited;

        private bool _isEditing;

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }

        private void Start()
        {
            _input.gameObject.SetActive(false);
            _editButton.onClick.AddListener(StartEdit);
        }

        public void SetText(string newText)
        {
            _text.text = newText;
        }

        public void SetInteractable(bool interactable)
        {
            _editButton.interactable = interactable;
        }

        private async void StartEdit()
        {
            _editButton.interactable = false;
            var ct = _cts.Token;
            var oldText = _text.text;
            var newText = await EditText(ct);

            bool changed = oldText != newText;

            if (changed)
            {
                _onEdited.Invoke(newText);
            }
            
            _editButton.interactable = true;
        }

        private async Task<string> EditText(CancellationToken ct)
        {
            _text.gameObject.SetActive(false);
            _input.gameObject.SetActive(true);
            _input.text = _text.text;

            _isEditing = true;
            _input.onEndEdit.AddListener(OnEndEdit);
            await WaitEdit(ct);
            _input.onEndEdit.RemoveListener(OnEndEdit);
            
            _text.gameObject.SetActive(true);
            _input.gameObject.SetActive(false);

            var newText = _input.text;
            _text.text = newText;
            return newText;
        }

        private void OnEndEdit(string value)
        {
            _isEditing = false;
        }

        private async Task WaitEdit(CancellationToken ct)
        {
            while (_isEditing && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }
    }
}