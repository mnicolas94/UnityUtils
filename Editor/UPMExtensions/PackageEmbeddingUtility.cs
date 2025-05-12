using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine.UIElements;
using Utils.Editor;
using Debug = UnityEngine.Debug;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using Path = System.IO.Path;

namespace Utils.Editor.PackageEmbed
{
    [InitializeOnLoad]
    public class PackageEmbeddingUtility : IPackageManagerExtension
    {
        static PackageEmbeddingUtility()
        {
            PackageManagerExtensions.RegisterExtension(new PackageEmbeddingUtility());
        }

        private Button _embedButton;
        private Button _deembedButton;
        private Button _goto;
        private Label _progressLabel;
    
        private bool _selectedIsFromGit;
        private string _selectedUrl;
        private string _selectedName;
        private bool _isProcessing;
        private VisualElement _root;

        private string AbsolutePackagesPath => Path.Join(Path.GetFullPath("."), "Packages");
        private string EmbeddedPackagePath => Path.Join(AbsolutePackagesPath, _selectedName);
        
        private bool IsEmbedded
        {
            get
            {
                var packages = Directory.GetDirectories("Packages");
                var selectedPath = Path.Join("Packages", _selectedName);
                return packages.Contains(selectedPath);
            }
        }

        public VisualElement CreateExtensionUI()
        {
            _embedButton = new Button(EmbedSelected)
            {
                text = "Embed",
            };
            _deembedButton = new Button(DeEmbedSelected)
            {
                text = "De-Embed",
            };
            _goto = new Button(GoToEmbedDir)
            {
                text = "Go to Folder",
            };

            _progressLabel = new Label("Embedding ...");
            _progressLabel.visible = false;
        
            _root = new VisualElement();
            _root.style.flexDirection = FlexDirection.Row;
            _root.Add(_embedButton);
            _root.Add(_deembedButton);
            _root.Add(_goto);
            _root.Add(_progressLabel);
        
            return _root;
        }

        private void UpdateUi()
        {
            var isEmbedded = IsEmbedded;
            var showRoot = _selectedIsFromGit || isEmbedded;
            _root.style.display = showRoot ? DisplayStyle.Flex : DisplayStyle.None;
            _embedButton.style.display = _selectedIsFromGit && !isEmbedded ? DisplayStyle.Flex : DisplayStyle.None;
            _deembedButton.style.display = isEmbedded ? DisplayStyle.Flex : DisplayStyle.None;
            _goto.style.display = isEmbedded ? DisplayStyle.Flex : DisplayStyle.None;
            _embedButton.SetEnabled(!_isProcessing);
            _deembedButton.SetEnabled(!_isProcessing);
            _progressLabel.visible = _isProcessing;
        }

        public void OnPackageSelectionChange(PackageInfo packageInfo)
        {
            if (packageInfo == null || _isProcessing)
            {
                return;
            }
        
            _selectedName = packageInfo.name;
            _selectedIsFromGit = packageInfo.git != null;
            if (_selectedIsFromGit)
            {
                _selectedUrl = packageInfo.repository.url;
            }

            UpdateUi();
        }

        public void OnPackageAddedOrUpdated(PackageInfo packageInfo)
        {
        }

        public void OnPackageRemoved(PackageInfo packageInfo)
        {
        }

        private async void EmbedSelected()
        {
            if (_selectedIsFromGit && !_isProcessing && !IsEmbedded)
            {
                _isProcessing = true;
                UpdateUi();
                
                // clone inside Packages folder
                var command = "git";
                var options = $"clone {_selectedUrl} ./{_selectedName}";
                var workingDir = "./Packages";
                try
                {
                    await RunCommand(command, options, workingDir);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
                finally
                {
                    _isProcessing = false;
                    UpdateUi();
                }
            }
        }

        private async void DeEmbedSelected()
        {
            if (IsEmbedded && !_isProcessing)
            {
                _isProcessing = true;
                UpdateUi();

                var delete = true;
                var hasChanges = await HasChanges();
                if (hasChanges)
                {
                    delete = EditorInputDialog.ShowYesNoDialog(
                        "WARNING",
                        "The embedded package you want to de-embed contains uncommited or un-pushed changes. " +
                        "You will lose those changes if you continue. Are you sure you want to de-embed this package?"
                    );
                }

                if (delete)
                {
                    // Remove the embedded package with a command because Directory.Delete(path, true) has issues
                    // with .git folder being hidden and files within it being read-only. Also, the symlinks Unity creates
                    // between Packages folder and Library/PackageCache makes it difficult to find a workaround.
                    await RunCommand("cmd.exe", $"/C rmdir /q /s \"{EmbeddedPackagePath}\"", "");
                }
                
                _isProcessing = false;
                UpdateUi();
            }
        }

        private void GoToEmbedDir()
        {
            EditorUtility.RevealInFinder(EmbeddedPackagePath);
        }
        
        private async Task<bool> HasChanges()
        {
            var hasUncommittedChanges = await HasUncommittedChanges(EmbeddedPackagePath);
            var hasUnPushedChanges = await HasUnPushedCommits(EmbeddedPackagePath);

            return hasUncommittedChanges || hasUnPushedChanges;
        }
        
        private static async Task<bool> HasUncommittedChanges(string repoPath)
        {
            try
            {
                var (output, _) = await RunCommand("git", "status --porcelain", repoPath);
                return !string.IsNullOrWhiteSpace(output);
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> HasUnPushedCommits(string repoPath)
        {
            try
            {
                var (output, _) = await RunCommand("git", "rev-list --count @{u}..", repoPath);
                return int.TryParse(output.Trim(), out int count) && count > 0;
            }
            catch
            {
                // Probably no upstream is set
                return false;
            }
        }

        private static async Task<(string, string)> RunCommand(string command, string options, string workingDir)
        {
            // Set up our processInfo to run the command and log to output and errorOutput.
            ProcessStartInfo processInfo = new ProcessStartInfo(command, options)
            {
                WorkingDirectory = workingDir,
                CreateNoWindow = true, // We want no visible pop-ups
                UseShellExecute = false, // Allows us to redirect input, output and error streams
                RedirectStandardOutput = true, // Allows us to read the output stream
                RedirectStandardError = true // Allows us to read the error stream
            };

            // Set up the Process
            Process process = new Process
            {
                StartInfo = processInfo
            };
            process.Start();
            
            // Read the results back from the process so we can get the output and check for errors
            var output = await process.StandardOutput.ReadToEndAsync();
            var errorOutput = await process.StandardError.ReadToEndAsync();

            while (!process.HasExited)
            {
                await Task.Yield();
            }
            bool hadErrors = process.ExitCode != 0;
            process.Close();  // Close the process ensuring it frees it resources.

            if (hadErrors)
            {
                throw new Exception($"{output}\n{errorOutput}");
            }

            return (output, errorOutput);
        }
    }
}