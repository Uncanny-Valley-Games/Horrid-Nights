using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WoodCuttingMinigame : MonoBehaviour
{
    public static WoodCuttingMinigame Instance { get; private set; }

    [Header("UI")] 
    [SerializeField] public GameObject minigameUI; // Canvas or root GameObject for the minigame
    [SerializeField] public RectTransform bar; // The long vertical box
    [SerializeField] public RectTransform arrow; // Arrow that moves vertically inside bar

    [Header("Chop Area")]
    [SerializeField] public RectTransform chopArea; // RectTransform for the chopping area (child of bar). Start disabled.
    [SerializeField] public Image chopAreaFill; // Image inside chopArea (optional) to show fill
    [SerializeField] public float maxChopAreaHeight = 40f; // pixels (max area height)
    [SerializeField] public float fillPerChop = 0.25f; // amount added to chopFill per successful chop
    private float initialChopAreaHeight; // pixels (minimum area height)

    [Header("Gameplay")] 
    [SerializeField] public float arrowSpeed = 300f; // pixels per second
    [SerializeField] public bool arrowMovesUpFirst = true;
    [SerializeField] public float chopCooldown;
    private bool active = false;
    private WoodCuttingInteract currentTree;
    private float arrowDirection = 1f; // 1 up, -1 down

    // chop area state
    private bool areaCreated = false;
    private float chopFill = 0f; // 0..1

    private float timer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (minigameUI != null) minigameUI.SetActive(false);
        if (chopArea != null) chopArea.gameObject.SetActive(false);
        if (chopAreaFill != null) chopAreaFill.fillAmount = 0f;

        initialChopAreaHeight = Random.Range(0f, maxChopAreaHeight);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (!active) return;

        MoveArrow();

        bool chop = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) chop = true;

        if (chop && timer >= chopCooldown)
        {
            TryChop();
            timer = 0f;
        }
    }

    public void StartMinigame(WoodCuttingInteract tree)
    {
        if (active) return;
        currentTree = tree;

        if (minigameUI != null) minigameUI.SetActive(true);

        // initialize arrow position and direction
        arrowDirection = arrowMovesUpFirst ? 1f : -1f;
        if (arrow != null && bar != null)
        {
            Vector2 anchored = arrow.anchoredPosition;
            anchored.y = (arrowDirection > 0) ? -bar.rect.height * 0.45f : bar.rect.height * 0.45f;
            arrow.anchoredPosition = anchored;
        }

        // reset chop area state
        areaCreated = false;
        chopFill = 0f;
        if (chopArea != null) chopArea.gameObject.SetActive(false);
        if (chopAreaFill != null) chopAreaFill.fillAmount = 0f;

        active = true;
    }

    void MoveArrow()
    {
        if (arrow == null || bar == null) return;

        Vector2 pos = arrow.anchoredPosition;
        pos.y += arrowDirection * arrowSpeed * Time.deltaTime;

        float half = bar.rect.height * 0.5f;
        float topLimit = half * 0.9f;
        float bottomLimit = -half * 0.9f;

        if (pos.y > topLimit)
        {
            pos.y = topLimit;
            arrowDirection = -1f;
        }
        else if (pos.y < bottomLimit)
        {
            pos.y = bottomLimit;
            arrowDirection = 1f;
        }

        arrow.anchoredPosition = pos;
    }

    void TryChop()
    {
        if (arrow == null || bar == null) return;

        float arrowY = arrow.anchoredPosition.y;

        if (!areaCreated)
        {
            CreateChopAreaAt(arrowY);
            IncreaseChopFill(fillPerChop);
            return;
        }

        // check if arrow is inside the chopArea vertical bounds
        if (chopArea != null)
        {
            float areaY = chopArea.anchoredPosition.y;
            float half = chopArea.rect.height * 0.5f;
            if (Mathf.Abs(arrowY - areaY) <= half)
            {
                IncreaseChopFill(fillPerChop);
            }
            else
            {
                // miss: optional feedback could be added here
            }
        }
    }

    void CreateChopAreaAt(float y)
    {
        if (chopArea == null || bar == null) return;

        // ensure pivot is centered so size changes expand equally up/down unless clamped
        chopArea.pivot = new Vector2(0.5f, 0.5f);

        // place the chop area at the arrow Y (local to bar)
        Vector2 anchored = chopArea.anchoredPosition;
        anchored.y = Mathf.Clamp(y, -bar.rect.height * 0.5f, bar.rect.height * 0.5f);
        chopArea.anchoredPosition = anchored;

        // give it an initial height (use SetSizeWithCurrentAnchors to avoid layout interference)
        float initH = Mathf.Max(1f, initialChopAreaHeight);
        chopArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initH);

        chopFill = 0f;
        if (chopAreaFill != null) chopAreaFill.fillAmount = 0f;

        chopArea.gameObject.SetActive(true);
        areaCreated = true;
    }

    void IncreaseChopFill(float amount)
    {
        if (chopArea == null || bar == null) return;

        chopFill = Mathf.Clamp01(chopFill + amount);

        // update optional visual fill inside the chop area
        if (chopAreaFill != null) chopAreaFill.fillAmount = chopFill;

        // compute sizes and positions in bar-local space
        float currentH = chopArea.rect.height;
        float barTop = bar.rect.height * 0.5f;
        float barBottom = -barTop;

        float pivotY = chopArea.pivot.y;
        float anchoredY = chopArea.anchoredPosition.y;
        float areaTop = anchoredY + currentH * (1f - pivotY);
        float areaBottom = anchoredY - currentH * pivotY;

        // desired new total height based on fill
        float maxHeight = bar.rect.height; // full bar height (no padding so top & bottom can be reached)
        float targetH = Mathf.Lerp(initialChopAreaHeight, maxHeight, chopFill);
        float deltaH = Mathf.Max(0f, targetH - currentH);

        if (deltaH <= Mathf.Epsilon)
        {
            // still update finish check below
            CheckFinish(areaTop, areaBottom);
            return;
        }

        // attempt to split growth equally
        float wantTop = deltaH * 0.5f;
        float wantBottom = deltaH * 0.5f;

        float availableTop = Mathf.Max(0f, barTop - areaTop);
        float availableBottom = Mathf.Max(0f, areaBottom - barBottom);

        float actualTop = Mathf.Min(wantTop, availableTop);
        float actualBottom = Mathf.Min(wantBottom, availableBottom);

        float remaining = deltaH - (actualTop + actualBottom);

        // distribute remaining to sides that still have space (try top then bottom)
        if (remaining > 0f)
        {
            float extraTop = Mathf.Min(remaining, Mathf.Max(0f, availableTop - actualTop));
            actualTop += extraTop;
            remaining -= extraTop;
        }

        if (remaining > 0f)
        {
            float extraBottom = Mathf.Min(remaining, Mathf.Max(0f, availableBottom - actualBottom));
            actualBottom += extraBottom;
            remaining -= extraBottom;
        }

        // If there is still remaining (no space on either side), clamp to available sum (no-op)
        float finalTop = areaTop + actualTop;
        float finalBottom = areaBottom - actualBottom;

        float finalHeight = finalTop - finalBottom;
        float finalAnchoredY = (finalTop + finalBottom) * 0.5f;

        // apply
        chopArea.anchoredPosition = new Vector2(chopArea.anchoredPosition.x, finalAnchoredY);
        chopArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(1f, finalHeight));

        // check finish (both sides reached)
        CheckFinish(finalTop, finalBottom);
    }

    void CheckFinish(float areaTop, float areaBottom)
    {
        float barTop = bar.rect.height * 0.5f;
        float barBottom = -barTop;
        float eps = 0.5f;

        bool topReached = areaTop >= barTop - eps;
        bool bottomReached = areaBottom <= barBottom + eps;

        if (topReached && bottomReached)
        {
            FinishMinigame(true);
        }
    }

    public void FinishMinigame(bool success)
    {
        if (!active) return;
        active = false;

        if (minigameUI != null) minigameUI.SetActive(false);
        if (chopArea != null) chopArea.gameObject.SetActive(false);

        if (success && currentTree != null)
        {
            currentTree.BreakTree();
        }

        currentTree = null;
        areaCreated = false;
        chopFill = 0f;
    }
}