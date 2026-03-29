using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugStuff
{
    public class DebugTextView : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        
        private InputAction debugKey;
        
        private TextMeshProUGUI _text;
        private SortedDictionary<string, string> _debugData = new SortedDictionary<string, string>();
        
        public float pollingTime = 1f; // time to update the debug text

        private float time; // delta time count
        private int frameCount; // frame rate counter

        private bool updateDebugTextFlag;
        
        private bool showDebugText;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            
            debugKey = playerInput.actions["Debug"];
            
            _debugData.Add("Current FPS", Application.targetFrameRate.ToString());
            _debugData.Add("Target FPS", Application.targetFrameRate.ToString());
            _debugData.Add("Application Name", Application.productName);
            _debugData.Add("Company Name", Application.companyName);
            _debugData.Add("Application Version", Application.version);
            _debugData.Add("Unity Version", Application.unityVersion);
            _debugData.Add("Current Build GUID", Application.buildGUID);
        }

        // Update is called once per frame
        private void Update()
        {
            if (debugKey.WasPressedThisFrame())
            {
                showDebugText = !showDebugText;
                
                updateDebugTextFlag = showDebugText; // updates when showDebugText is set to true for the first frame
            }
            
            if (!showDebugText)
            {
                _text.text = "";
                return;
            }
            
            // everything from this point on will only run if _showDebugText is true
            
            time += Time.deltaTime;

            if (time > pollingTime)
            {
                time -= pollingTime;
                
                _debugData["Current FPS"] = Mathf.RoundToInt(1 / Time.unscaledDeltaTime).ToString();

                updateDebugTextFlag = true;
            }

            if (updateDebugTextFlag)
            {
                _text.text = "";
            
                foreach (KeyValuePair<string, string> kvp in _debugData)
                {
                    _text.text += kvp.Key + ": " + kvp.Value + "\n";
                }
                
                updateDebugTextFlag = false;
            }
        }
    }
}
