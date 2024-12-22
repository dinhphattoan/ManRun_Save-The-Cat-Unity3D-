using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIScript : MonoBehaviour
{
    [SerializeField] private Image panelImageTappingUpgrade;
    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private TextMeshProUGUI currencyTextShow;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        currencyTextShow.text = mainMenuManager.GameDatabase.CurrencyValue.ToString();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0 && Application.isMobilePlatform)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;
            HandleClick(touchPosition);
        }
        else if (Input.mousePosition != null)
        {
            Vector2 mousePosition = Input.mousePosition;
            HandleClick(mousePosition);
        }
    }

    private void HandleClick(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelImageTappingUpgrade.rectTransform,
            position,
            Camera.main,
            out Vector2 localPoint);

        if (localPoint != Vector2.zero)
        {
            mainMenuManager.HandleScreenTap(localPoint);
        }
    }

    public void HandleUpgrade()
    {
        mainMenuManager.HandleUpgradeTap();
        currencyTextShow.text = mainMenuManager.GameDatabase.CurrencyValue.ToString();
    }

    private void Update()
    {
        // HandleInput(); // Assuming input handling should happen every frame
    }
}