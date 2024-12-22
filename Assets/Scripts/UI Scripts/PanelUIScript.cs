using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIScript : MonoBehaviour
{
    [SerializeField] private Image panelImage;
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private TextMeshProUGUI currencyText;
    Vector2 localPoint = Vector2.zero;
    private void OnEnable()
    {
        Initialize();
    }
    public void Initialize()
    {
        // panelImage.GetComponent<Button>().onClick.AddListener(HandleClick);
        currencyText.text = mainMenuManager.GameDatabase.CurrencyValue.ToString();
    }
    public void HandleClick()
    {
        localPoint = Vector2.zero;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = touch.position;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    panelImage.rectTransform,
                    touchPosition,
                    Camera.main,
                    out localPoint
                );
            }
        }
        else
        {
            Vector2 mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelImage.rectTransform,
                mousePosition,
                Camera.main,
                out localPoint
            );

        }
        if (localPoint != Vector2.zero)
        {
            mainMenuManager.HandleScreenTap(localPoint);
        }
    }
    public void HandleUpgrade()
    {
        mainMenuManager.HandleUpgradeTap();
        currencyText.text = mainMenuManager.GameDatabase.CurrencyValue.ToString();
    }
}
