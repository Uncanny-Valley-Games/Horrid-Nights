using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UpdatableText : MonoBehaviour
{
    private static string _statusText;
    
    private static string _previousStatusText;

    public static void UpdateStatusText(string text)
    {
        _statusText = text;
    }

    [SerializeField] private float showTime = 5;
    
    private TextMeshProUGUI _textMeshProUGUI;

    private void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _textMeshProUGUI.enabled = false;
    }

    private void Update()
    {
        if (_previousStatusText != _statusText)
        {
            _previousStatusText = _statusText;
            StartCoroutine(ShowText());
        }
    }

    IEnumerator ShowText()
    {
        _textMeshProUGUI.text = _statusText;
        _textMeshProUGUI.enabled = true;
        yield return new WaitForSeconds(showTime);
        _textMeshProUGUI.enabled = false;
    }
}
