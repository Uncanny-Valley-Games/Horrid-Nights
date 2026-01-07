using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Mini_Games.WoodCutting
{
    public class WoodCuttingMinigame : MonoBehaviour
    {
        public static WoodCuttingMinigame Instance { get; private set; }

        [Header("UI")] [SerializeField] public GameObject minigameUI;
        [SerializeField] public RectTransform tree;
        [SerializeField] public RectTransform arrow;

        [Header("Chop Area")] [SerializeField] public RectTransform chopArea;
        [SerializeField] public Image chopAreaFill;
        [SerializeField] public float maxChopAreaHeight = 40f;
        [SerializeField] public float fillPerChop = 0.25f;
        private float _initialChopAreaHeight;

        [Header("Gameplay")] [SerializeField] public float arrowSpeed = 300f;
        [SerializeField] public bool arrowMovesUpFirst = true;
        [SerializeField] public float chopCooldown;
        private bool _active;
        private WoodCuttingInteract _currentTree;
        private float _arrowDirection = 1f;

        private bool _areaCreated;
        private float _chopFill;

        private float _timer;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (minigameUI != null) minigameUI.SetActive(false);
            if (chopArea != null) chopArea.gameObject.SetActive(false);
            if (chopAreaFill != null) chopAreaFill.fillAmount = 0f;

            _initialChopAreaHeight = Random.Range(0f, maxChopAreaHeight);
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (!_active) return;

            MoveArrow();

            bool chop = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
                        || Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

            if (chop && _timer >= chopCooldown)
            {
                TryChop();
                _timer = 0f;
            }
        }

        public void StartMinigame(WoodCuttingInteract currentTree)
        {
            if (_active) return;
            _currentTree = currentTree;

            if (minigameUI) minigameUI.SetActive(true);

            _arrowDirection = arrowMovesUpFirst ? 1f : -1f;
            if (arrow && tree)
            {
                Vector2 anchored = arrow.anchoredPosition;
                anchored.y = (_arrowDirection > 0) ? -tree.rect.height * 0.45f : tree.rect.height * 0.45f;
                arrow.anchoredPosition = anchored;
            }

            _areaCreated = false;
            _chopFill = 0f;
            if (chopArea) chopArea.gameObject.SetActive(false);
            if (chopAreaFill) chopAreaFill.fillAmount = 0f;

            _active = true;
        }

        void MoveArrow()
        {
            if (!arrow || !tree) return;

            Vector2 pos = arrow.anchoredPosition;
            pos.y += _arrowDirection * arrowSpeed * Time.deltaTime;

            float half = tree.rect.height * 0.5f;
            float topLimit = half * 0.9f;
            float bottomLimit = -half * 0.9f;

            if (pos.y > topLimit)
            {
                pos.y = topLimit;
                _arrowDirection = -1f;
            }
            else if (pos.y < bottomLimit)
            {
                pos.y = bottomLimit;
                _arrowDirection = 1f;
            }

            arrow.anchoredPosition = pos;
        }

        void TryChop()
        {
            if (!arrow || !tree) return;

            float arrowY = arrow.anchoredPosition.y;

            if (!_areaCreated)
            {
                CreateChopAreaAt(arrowY);
                IncreaseChopFill(fillPerChop);
                return;
            }

            if (chopArea)
            {
                float areaY = chopArea.anchoredPosition.y;
                float half = chopArea.rect.height * 0.5f;
                if (Mathf.Abs(arrowY - areaY) <= half)
                {
                    IncreaseChopFill(fillPerChop);
                }
            }
        }

        void CreateChopAreaAt(float y)
        {
            if (!chopArea || !tree) return;

            chopArea.pivot = new Vector2(0.5f, 0.5f);

            Vector2 anchored = chopArea.anchoredPosition;
            anchored.y = Mathf.Clamp(y, -tree.rect.height * 0.5f, tree.rect.height * 0.5f);
            chopArea.anchoredPosition = anchored;

            float initH = Mathf.Max(1f, _initialChopAreaHeight);
            chopArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initH);

            _chopFill = 0f;
            if (chopAreaFill) chopAreaFill.fillAmount = 0f;

            chopArea.gameObject.SetActive(true);
            _areaCreated = true;
        }

        void IncreaseChopFill(float amount)
        {
            if (!chopArea || !tree) return;

            _chopFill = Mathf.Clamp01(_chopFill + amount);

            if (chopAreaFill) chopAreaFill.fillAmount = _chopFill;

            float currentH = chopArea.rect.height;
            float barTop = tree.rect.height * 0.5f;
            float barBottom = -barTop;

            float pivotY = chopArea.pivot.y;
            float anchoredY = chopArea.anchoredPosition.y;
            float areaTop = anchoredY + currentH * (1f - pivotY);
            float areaBottom = anchoredY - currentH * pivotY;

            float maxHeight = tree.rect.height;
            float targetH = Mathf.Lerp(_initialChopAreaHeight, maxHeight, _chopFill);
            float deltaH = Mathf.Max(0f, targetH - currentH);

            if (deltaH <= Mathf.Epsilon)
            {
                CheckFinish(areaTop, areaBottom);
                return;
            }

            float wantTop = deltaH * 0.5f;
            float wantBottom = deltaH * 0.5f;

            float availableTop = Mathf.Max(0f, barTop - areaTop);
            float availableBottom = Mathf.Max(0f, areaBottom - barBottom);

            float actualTop = Mathf.Min(wantTop, availableTop);
            float actualBottom = Mathf.Min(wantBottom, availableBottom);

            float remaining = deltaH - (actualTop + actualBottom);

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
            }

            float finalTop = areaTop + actualTop;
            float finalBottom = areaBottom - actualBottom;

            float finalHeight = finalTop - finalBottom;
            float finalAnchoredY = (finalTop + finalBottom) * 0.5f;

            chopArea.anchoredPosition = new Vector2(chopArea.anchoredPosition.x, finalAnchoredY);
            chopArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(1f, finalHeight));

            CheckFinish(finalTop, finalBottom);
        }

        void CheckFinish(float areaTop, float areaBottom)
        {
            float barTop = tree.rect.height * 0.5f;
            float barBottom = -barTop;
            float eps = 0.5f;

            bool topReached = areaTop >= barTop - eps;
            bool bottomReached = areaBottom <= barBottom + eps;

            if (topReached && bottomReached)
            {
                FinishMinigame(true);
            }
        }

        private void FinishMinigame(bool success)
        {
            if (!_active) return;
            _active = false;

            if (minigameUI) minigameUI.SetActive(false);
            if (chopArea) chopArea.gameObject.SetActive(false);

            if (success && _currentTree)
            {
                _currentTree.BreakTree();
            }

            _currentTree = null;
            _areaCreated = false;
            _chopFill = 0f;
        }
    }
}