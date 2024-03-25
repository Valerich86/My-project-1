
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuControl : MonoBehaviour
{
    public void NewGamePressed() => SceneManager.LoadScene("SampleScene");

    public void ExitPressed() => Application.Quit();
}
