using UnityEngine;
using UnityEngine.SceneManagement;

namespace Downscaled
{
    public class DMainMenuManager : MonoBehaviour
    {
        public void SwitchToGameplay()
        {
            DGameManager.ResetVariables();
            SceneManager.LoadScene("DGameplay");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
