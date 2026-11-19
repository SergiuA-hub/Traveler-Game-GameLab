using System.Diagnostics;
using System.IO;
using UnityEngine;

public class QuickRestart : MonoBehaviour
{
    private bool restartRequested;

    public void RestartGame()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (restartRequested)
            return;

        restartRequested = true;

        try
        {
            using var currentProcess = Process.GetCurrentProcess();
            string executablePath = currentProcess.MainModule.FileName;

            using var newProcess = Process.Start(new ProcessStartInfo
            {
                FileName = executablePath,
                WorkingDirectory = Path.GetDirectoryName(executablePath),
                UseShellExecute = false
            });

            if (newProcess == null)
                throw new System.InvalidOperationException("Game process did not start.");

            Application.Quit();
        }
        catch (System.Exception exception)
        {
            restartRequested = false;
            UnityEngine.Debug.LogException(exception);
        }
#else
        UnityEngine.Debug.Log("Testează Restart Game într-un build de Windows.");
#endif
    }
}