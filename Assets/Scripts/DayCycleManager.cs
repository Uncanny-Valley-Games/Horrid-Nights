using System;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    public enum DayState
    {
        Day,
        Evening,
        Night
    }

    private static DayCycleManager Instance { get; set; }
    public static event Action<DayState> OnDayStateChanged;

    [SerializeField] private DayState startState = DayState.Day;
    [SerializeField] private bool autoCycle;
    [SerializeField] private float cycleDuration = 60f; // seconds per state when autoCycle is true

    private DayState _currentState;
    private float _timer;

    public DayState CurrentState => _currentState;
    public bool IsDay => _currentState == DayState.Day;
    public bool IsEvening => _currentState == DayState.Evening;
    public bool IsNight => _currentState == DayState.Night;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetState(startState, true);
    }

    void Update()
    {
        if (!autoCycle) return;

        _timer += Time.deltaTime;
        if (_timer >= cycleDuration)
        {
            ToggleState();
            _timer = 0f;
        }
    }

    private void SetState(DayState state, bool forceNotify = false)
    {
        if (_currentState == state && !forceNotify) return;

        _currentState = state;
        OnDayStateChanged?.Invoke(_currentState);
        ApplyEnvironmentForState(_currentState);
    }

    private void ToggleState()
    {
        switch (_currentState)
        {
            case DayState.Day:
                SetState(DayState.Evening);
                break;
            case DayState.Evening:
                SetState(DayState.Night);
                break;
            case DayState.Night:
                SetState(DayState.Day);
                break;
            default:
                SetState(startState);
                break;
        }
    }

    private void ApplyEnvironmentForState(DayState state)
    {
        if (state == DayState.Day)
        {
            RenderSettings.ambientLight = Color.white * 0.9f;
        }
        else if (state == DayState.Evening)
        {
            RenderSettings.ambientLight = Color.Lerp(Color.white * 0.9f, Color.gray * 0.25f, 0.5f);
        }
        else
        {
            RenderSettings.ambientLight = Color.gray * 0.25f;
        }
    }
}