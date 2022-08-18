﻿using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


[InitializeOnLoad]
public class GitUpdateButtonUpmExtension : IPackageManagerExtension
{
    static GitUpdateButtonUpmExtension()
    {
        PackageManagerExtensions.RegisterExtension(new GitUpdateButtonUpmExtension());
    }

    private Button _button;
    private Label _progressLabel;
    
    private bool _selectedIsFromGit;
    private string _selectedUrl;
    private AddRequest _addRequest;

    public VisualElement CreateExtensionUI()
    {
        _button = new Button(UpdateSelected)
        {
            text = "Update",
            style =
            {
                alignSelf = Align.FlexStart,
            }
        };
        _button.visible = false;

        _progressLabel = new Label();
        _progressLabel.visible = false;
        
        var root = new VisualElement();
        root.style.flexDirection = FlexDirection.Row;
        root.Add(_button);
        root.Add(_progressLabel);
        
        return root;
    }

    public void OnPackageSelectionChange(PackageInfo packageInfo)
    {
        if (packageInfo == null)
        {
            return;
        }
        
        _selectedIsFromGit = packageInfo.git != null;
        if (_selectedIsFromGit)
        {
            _selectedUrl = packageInfo.repository.url;
        }
        
        _button.visible = _selectedIsFromGit;
    }

    public void OnPackageAddedOrUpdated(PackageInfo packageInfo)
    {
    }

    public void OnPackageRemoved(PackageInfo packageInfo)
    {
    }

    private void UpdateSelected()
    {
        if (_selectedIsFromGit)
        {
            _addRequest = Client.Add(_selectedUrl);
            SetUpdatingState(true);
            EditorApplication.update += Progress;
        }
    }

    private void SetUpdatingState(bool updating)
    {
        _button.SetEnabled(!updating);
        _progressLabel.visible = updating;
    }
    
    private void Progress()
    {
        UpdateProgressUi();
        if (_addRequest.IsCompleted)
        {
            if (_addRequest.Status == StatusCode.Success)
                Debug.Log("Installed: " + _addRequest.Result.packageId);
            else if (_addRequest.Status >= StatusCode.Failure)
                Debug.Log(_addRequest.Error.message);

            SetUpdatingState(false);
            EditorApplication.update -= Progress;
        }
    }

    private void UpdateProgressUi()
    {
        int n = (int)EditorApplication.timeSinceStartup % 4;
        string dots = new string('.', n);
        _progressLabel.text = $"Updating{dots}";
    }
}