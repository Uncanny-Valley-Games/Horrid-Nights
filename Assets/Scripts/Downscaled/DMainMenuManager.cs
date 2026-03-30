using UnityEngine;
using UnityEngine.SceneManagement;

namespace Downscaled
{
    public class DMainMenuManager : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
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
