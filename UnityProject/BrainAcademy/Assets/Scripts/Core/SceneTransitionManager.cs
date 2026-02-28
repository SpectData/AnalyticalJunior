using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadDifficultySelect()
    {
        SceneManager.LoadScene("DifficultySelectScene");
    }

    public void LoadSnakeSpell()
    {
        SceneManager.LoadScene("SnakeSpellScene");
    }

    public void LoadSnakeResults()
    {
        SceneManager.LoadScene("SnakeSpellResultsScene");
    }
}
