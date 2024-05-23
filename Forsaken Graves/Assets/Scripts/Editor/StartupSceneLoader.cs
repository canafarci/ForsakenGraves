using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace ForsakenGraves.Editor
{
    [InitializeOnLoad]
    public class StartupSceneLoader
    {
        private const string PREVIOUS_SCENE_KEY = "PreviousScene";
        private const string SHOULD_LOAD_STARTUP_SCENE_KEY = "LoadStartupScene";

        private const string LOAD_STARTUP_SCENE_ON_PLAY = "Development/Load Startup Scene On Play";
        private const string DONT_LOAD_STARTUP_SCENE_ON_PLAY = "Development/Don't Load Startup Scene On Play";

        private static bool _restartingToSwitchedScene;
        
        private static string StartupScene => EditorBuildSettings.scenes[0].path;
        
        static StartupSceneLoader()
        {
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

#region  Getters-Setters

        private static string PreviousScene
        {
            get => EditorPrefs.GetString(PREVIOUS_SCENE_KEY);
            set => EditorPrefs.SetString(PREVIOUS_SCENE_KEY, value);
        }

        private static bool ShouldLoadStartupScene
        {
            get
            {
                if (!EditorPrefs.HasKey(SHOULD_LOAD_STARTUP_SCENE_KEY))
                {
                    EditorPrefs.SetBool(SHOULD_LOAD_STARTUP_SCENE_KEY, true);
                }

                return EditorPrefs.GetBool(SHOULD_LOAD_STARTUP_SCENE_KEY);
            }
            
            set => EditorPrefs.SetBool(SHOULD_LOAD_STARTUP_SCENE_KEY, value);
        }

#endregion
        
        [MenuItem(LOAD_STARTUP_SCENE_ON_PLAY, true)]
        static bool ShowLoadStartupSceneOnPlay()
        {
            return !ShouldLoadStartupScene;
        }

        [MenuItem(LOAD_STARTUP_SCENE_ON_PLAY)]
        static void EnableLoadStartupSceneOnPlay()
        {
            ShouldLoadStartupScene = true;
        }

        [MenuItem(DONT_LOAD_STARTUP_SCENE_ON_PLAY, true)]
        static bool ShowDoNotLoadStartupSceneOnPlay()
        {
            return ShouldLoadStartupScene;
        }

        [MenuItem(DONT_LOAD_STARTUP_SCENE_ON_PLAY)]
        static void DisableDoNotLoadBootstrapSceneOnPlay()
        {
            ShouldLoadStartupScene = false;
        }
        
        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (!ShouldLoadStartupScene) return;

            if (_restartingToSwitchedScene) //error check as multiple starts and stops happening
            {
                if (playModeStateChange == PlayModeStateChange.EnteredPlayMode)
                {
                    _restartingToSwitchedScene = false;
                }
                return;
            }

            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                // cache previous scene to return to it after play session ends
                PreviousScene = EditorSceneManager.GetActiveScene().path;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // user either hit "Save" or "Don't Save"; open bootstrap scene

                    if (!string.IsNullOrEmpty(StartupScene) && System.Array.Exists(EditorBuildSettings.scenes, scene => scene.path == StartupScene))
                    {
                        Scene activeScene = EditorSceneManager.GetActiveScene();

                        _restartingToSwitchedScene = activeScene.path == string.Empty || !StartupScene.Contains(activeScene.path);

                        // only switch if editor is in a empty scene or active scene is not startup scene
                        if (_restartingToSwitchedScene)
                        {
                            EditorApplication.isPlaying = false;

                            // scene is included in build settings; open it
                            EditorSceneManager.OpenScene(StartupScene);

                            EditorApplication.isPlaying = true;
                        }
                    }
                }
                else
                {
                    // user either hit "Cancel" or exited window; don't open startup scene & return to editor
                    EditorApplication.isPlaying = false;
                }
            }
            //return to last open scene
            else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                if (!string.IsNullOrEmpty(PreviousScene))
                {
                    EditorSceneManager.OpenScene(PreviousScene);
                }
            }
        }
    }
}