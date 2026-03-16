using UnityEngine;
using UnityEngine.SceneManagement;

public class DMainMenuManager : MonoBehaviour
{
    public void SwitchToGameplay()
    {
        SceneManager.LoadScene("DGameplay");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
