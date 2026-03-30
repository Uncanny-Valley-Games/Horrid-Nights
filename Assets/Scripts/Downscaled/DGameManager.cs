using UnityEngine;

namespace Downscaled
{
    public class DGameManager : MonoBehaviour
    {
        public static bool TimerStarted = false;

        public static bool FishingMinigameDone = false;
    
        public static bool TreeCuttingMinigameDone = false;
    
        public static bool NailMinigameDone = false;

        public static void ResetVariables()
        {
            TimerStarted = false;
            FishingMinigameDone = false;
            TreeCuttingMinigameDone = false;
            NailMinigameDone = false;
        }
    }
}
