using UnityEditor;

namespace ForsakenGraves.Editor
{
    [InitializeOnLoad]
    public class StartupSceneLoader
    {
        private const string PREVIOUS_SCENE_KEY = "PreviousScene";
        private const string STARTUP_SCENE_KEY = "LoadStartupScene";
        
        private static string BootstrapScene => EditorBuildSettings.scenes[0].path;
        
        private static string PreviousScene
        {
            get => EditorPrefs.GetString(PREVIOUS_SCENE_KEY);
            set => EditorPrefs.SetString(PREVIOUS_SCENE_KEY, value);
        }
    }
}