using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private void Awake()
    {
        InitSingleton(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    private void ExitGame()
    {
        Debug.Log("Exiting Application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void InitSingleton(GameController instance)
    {
        if (!Instance)
        {
            Instance = instance;

            DontDestroyOnLoad(instance.gameObject);//IMMORTALITY!
        }
        else
        {
            Destroy(instance.gameObject);
        }

    }
}
