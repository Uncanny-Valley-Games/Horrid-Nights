using UnityEngine;
using UnityEngine.SceneManagement;

namespace Downscaled
{
    public class DarknessDoor : MonoBehaviour
    {
        private void Start()
        {
            DGameManager.ResetVariables();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!DGameManager.FinishedInitialMiniGames()) return;
            if (other.gameObject.CompareTag("Player"))
            {
                print("collision");
                SceneManager.LoadScene("DMenu");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!DGameManager.FinishedInitialMiniGames()) return;
            if (other.gameObject.CompareTag("Player"))
            {
                print("collision");
                SceneManager.LoadScene("DMenu");
            }
        }
    }
}
