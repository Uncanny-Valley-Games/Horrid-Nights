using UnityEngine;

namespace Downscaled
{
    public class DGameManager
    {
        public static bool TimerStarted = false;

        public static bool FishingMinigameDone = false;
    
        public static bool TreeCuttingMinigameDone = false;
    
        public static bool NailMinigameDone = false;
        
        public static float MouseSensitivity = 0.5f;

        public static void ResetVariables()
        {
            TimerStarted = false;
            FishingMinigameDone = false;
            TreeCuttingMinigameDone = false;
            NailMinigameDone = false;
        }
        
        public static bool FinishedInitialMiniGames()
        {
            return FishingMinigameDone && TreeCuttingMinigameDone;
        }
    }
}
