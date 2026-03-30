using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Downscaled
{
    public class DMainMenuManager : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        
        private void Start()
        {
            slider.value = DGameManager.MouseSensitivity;
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
        
        public void UpdateMouseSensitivity()
        {
            DGameManager.MouseSensitivity = slider.value;
        }
    }
}
