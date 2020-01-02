//using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;
//using UnityEngine.SceneManagement;

public class SceneMenu {

    public static void QueryOpenScene(string a_path)
    {
        if (EditorUtility.DisplayDialog("Save?", "Do you want to save the current scene?", "Make it so!", "Nope"))
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }
        EditorSceneManager.OpenScene(a_path);
    }

    [MenuItem("MegaScenes/BattleScene")]
    public static void OpenScene_BattleScene()
    {
        QueryOpenScene("Assets/Scenes/MegaMan Battle Scene.unity");

    }

    [MenuItem("MegaScenes/NaviSelect")]
    public static void OpenScene_NaviSelect()
    {
        QueryOpenScene("Assets/Scenes/Navi Select Screen.unity");

    }

    [MenuItem("MegaScenes/Tournament Screen")]
    public static void OpenScene_TournamentScreen()
    {
        QueryOpenScene("Assets/Scenes/Tournament Screen.unity");

    }
}


