using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Utils.Editor
{
    public static class GitUtils
    {
        public static string RunGitCommand(string gitCommand)
        {
            return RunGitCommand(gitCommand, "");
        }

        public static string RunGitCommand(string gitCommand, string workingDir) {
            // Strings that will catch the output from our process.
            string output = "no-git";
            string errorOutput = "no-git";

            // Set up our processInfo to run the git command and log to output and errorOutput.
            ProcessStartInfo processInfo = new ProcessStartInfo("git", @gitCommand) {
                WorkingDirectory = workingDir,
                CreateNoWindow = true,          // We want no visible pop-ups
                UseShellExecute = false,        // Allows us to redirect input, output and error streams
                RedirectStandardOutput = true,  // Allows us to read the output stream
                RedirectStandardError = true    // Allows us to read the error stream
            };

            // Set up the Process
            Process process = new Process {
                StartInfo = processInfo
            };
            try {
                process.Start();  // Try to start it, catching any exceptions if it fails
            } catch (Exception e) {
                // For now just assume its failed cause it can't find git.
                Debug.LogError("Git is not set-up correctly, required to be on PATH, and to be a git project.");
                throw e;
            }

            // Read the results back from the process so we can get the output and check for errors
            output = process.StandardOutput.ReadToEnd();
            errorOutput = process.StandardError.ReadToEnd();

            process.WaitForExit();  // Make sure we wait till the process has fully finished.
            process.Close();        // Close the process ensuring it frees it resources.

            // Check for failure due to no git setup in the project itself or other fatal errors from git.
            if (output.Contains("fatal") || output == "no-git") {
                throw new Exception("Command: git " + @gitCommand + " Failed\n" + output + errorOutput);
            }
            // Log any errors.
            if (errorOutput != "") {
                Debug.LogError("Git Error: " + errorOutput);
            }

            return output;  // Return the output from git.
        }

        public static string Add(string whatToAdd)
        {
            string gitCommand = $"add {whatToAdd}";
            return RunGitCommand(gitCommand);
        }
        
        public static string Commit(string message)
        {
            string gitCommand = $"commit -m {message}";
            return RunGitCommand(gitCommand);
        }
        
        public static string Push()
        {
            string gitCommand = "push";
            return RunGitCommand(gitCommand);
        }
        
        public static string Pull()
        {
            string gitCommand = "pull";
            return RunGitCommand(gitCommand);
        }
        
        public static string Restore()
        {
            string gitCommand = "restore .";
            return RunGitCommand(gitCommand);
        }

        public static string Switch(string switchTo)
        {
            string gitCommand = $"switch {switchTo}";
            return RunGitCommand(gitCommand);
        }
        
        public static string GetGitCommitHash()
        {
            string gitCommand = "rev-parse --short HEAD";
            var stdout = RunGitCommand(gitCommand);
            stdout = stdout.Trim();
            return stdout;
        }
        
        public static string GetLastTag()
        {
            string gitCommand = "describe --tags --abbrev=0 --match v[0-9]*";
            var stdout = RunGitCommand(gitCommand);
            stdout = stdout.Trim();
            return stdout;
        }

        public static string GetUserName()
        {
            string gitCommand = "config --get user.name";
            var stdout = RunGitCommand(gitCommand);
            stdout = stdout.Trim();
            return stdout;
        }
    }
}