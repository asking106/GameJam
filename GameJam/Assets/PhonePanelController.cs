using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhonePanelController : MonoBehaviour
{
    public RectTransform phonePanel;    
    public GameObject attributePanel;   
    public GameObject infoPanel;          
    public Button btnAttribute;          
    public Button btnInfo;                
    public float showAnimTime = 0.3f;     

    private bool isPanelVisible = false;
    private Vector2 hiddenPos;
    private Vector2 shownPos;
    private Coroutine animCoroutine;

    void Start()
    {
       
        shownPos = phonePanel.anchoredPosition;
        hiddenPos = new Vector2(shownPos.x, -phonePanel.rect.height);

        phonePanel.anchoredPosition = hiddenPos;
        phonePanel.gameObject.SetActive(false);

        attributePanel.SetActive(false);
        infoPanel.SetActive(false);

        btnAttribute.onClick.AddListener(ShowAttributePanel);
        btnInfo.onClick.AddListener(ShowInfoPanel);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (!isPanelVisible)
                ShowPhonePanel();
            else
                HidePhonePanel();
        }
    }

    void ShowPhonePanel()
    {
        phonePanel.gameObject.SetActive(true);
        isPanelVisible = true;
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PanelMoveAnim(hiddenPos, shownPos));
        Time.timeScale = 0f; // 暂停游戏
    }

    void HidePhonePanel()
    {
        isPanelVisible = false;
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PanelMoveAnim(phonePanel.anchoredPosition, hiddenPos, () => phonePanel.gameObject.SetActive(false)));
        Time.timeScale = 1f; // 恢复游戏
    }

    IEnumerator PanelMoveAnim(Vector2 from, Vector2 to, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < showAnimTime)
        {
            phonePanel.anchoredPosition = Vector2.Lerp(from, to, elapsed / showAnimTime);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        phonePanel.anchoredPosition = to;
        onComplete?.Invoke();
    }

    void ShowAttributePanel()
    {
        attributePanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    void ShowInfoPanel()
    {
        attributePanel.SetActive(false);
        infoPanel.SetActive(true);
    }
}