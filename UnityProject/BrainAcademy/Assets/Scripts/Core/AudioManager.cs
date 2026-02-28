using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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

    // Placeholder methods for future SFX
    public void PlayCorrect() { }
    public void PlayWrong() { }
    public void PlaySpellCast() { }
    public void PlaySnakeDeath() { }
    public void PlayWaveComplete() { }
    public void PlayGameOver() { }
    public void PlayButtonClick() { }
}
