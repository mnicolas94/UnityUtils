using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Utils.Editor
{
    public static class GitUtils
    {
        public static string RunGitCommandThrowException(string gitCommand, string workingDir)
        {
            var (output, errorOutput) = RunGitCommand(gitCommand, workingDir);
            if (errorOutput != "") {
                throw new Exception(errorOutput);
            }
            return output;
        }
        
        public static (string, string) RunGitCommand(string gitCommand)
        {
            return RunGitCommand(gitCommand, "");
        }

        public static (string, string) RunGitCommand(string gitCommand, string workingDir) {
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
                throw;
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

            return (output, errorOutput);  // Return the output from git.
        }

        public static string Add(string whatToAdd, string gitRoot = "")
        {
            string gitCommand = $"add {whatToAdd}";
            return RunGitCommandThrowException(gitCommand, gitRoot);
        }
        
        public static string Commit(string message, string gitRoot = "")
        {
            string gitCommand = $"commit -m {message}";
            return RunGitCommandThrowException(gitCommand, gitRoot);
        }
        
        public static string Push(string gitRoot = "")
        {
            string gitCommand = "push";
            return RunGitCommandThrowException(gitCommand, gitRoot);
        }
        
        public static string Pull(string gitRoot = "")
        {
            string gitCommand = "pull";
            return RunGitCommandThrowException(gitCommand, gitRoot);
        }
        
        public static string Restore(string whatToRestore, string gitRoot = "")
        {
            string gitCommand = $"restore {whatToRestore}";
            return RunGitCommandThrowException(gitCommand, gitRoot);
        }

        public static string Switch(string switchTo, string gitRoot = "")
        {
            string gitCommand = $"switch {switchTo}";
            var (output, errorOutput) = RunGitCommand(gitCommand, gitRoot);
            if (errorOutput.Contains("fatal"))
            {
                throw new Exception(errorOutput);
            }
            return output;
        }
        
        public static string GetGitCommitHash(string gitRoot = "")
        {
            string gitCommand = "rev-parse --short HEAD";
            var stdout = RunGitCommandThrowException(gitCommand, gitRoot);
            stdout = stdout.Trim();
            return stdout;
        }
        
        public static string GetLastTag(string gitRoot = "")
        {
            string gitCommand = "describe --tags --abbrev=0 --match v[0-9]*";
            var stdout = RunGitCommandThrowException(gitCommand, gitRoot);
            stdout = stdout.Trim();
            return stdout;
        }

        public static string GetUserName(string gitRoot = "")
        {
            string gitCommand = "config --get user.name";
            var stdout = RunGitCommandThrowException(gitCommand, gitRoot);
            stdout = stdout.Trim();
            return stdout;
        }
    }
}