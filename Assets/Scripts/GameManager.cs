
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    public TextMeshProUGUI Messager;
    public Canvas PauseCanvas;
    public int Level { get; private set; }
    public int CoinsNeeded { get; private set; }
    public int Score { get; private set; }
    public float HP { get; private set; }
    public int TorpedoQuantity { get; private set; }
    public float RotationSpeed { get; private set; }

    public bool IsPaused;

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Screen.fullScreen = true;

        instance = this;

        DontDestroyOnLoad(gameObject);

        Level = 0;

        CoinsNeeded = 3;

        Score = 0;

        HP = 1f;

        IsPaused = false;

        RotationSpeed = 70f;

        TorpedoQuantity = 0;

        PauseCanvas.gameObject.SetActive(false);

        LevelUp(Score, HP);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(IsPaused == false)
            {
                PauseCanvas.gameObject.SetActive(true);
                Messager.text = "перерыв!";
                Time.timeScale = 0;
                IsPaused = true;
            }
            else
            {
                PauseCanvas.gameObject.SetActive(false);
                Time.timeScale = 1;
                IsPaused = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public void LevelUp(int score, float hp)
    {
        Level += 1;
        Score = score;
        HP = hp;
        CoinsNeeded += 2;
        TorpedoQuantity += 1;
        SceneManager.LoadScene(0);
        Invoke(nameof(LoadMainScene), 5f);
    }

    private void LoadMainScene() => SceneManager.LoadScene(1);
   
    public void GameOverScene()
    {
        HP = 1f;
        Score = 0;
        Level = 1;
        TorpedoQuantity = 1;
        CoinsNeeded = 3;
        SceneManager.LoadScene(2);
    }
}
