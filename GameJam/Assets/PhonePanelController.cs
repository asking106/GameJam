using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.FantasyInventory.Scripts.Interface.Elements;

public class PhonePanelController : MonoBehaviour
{
    public RectTransform phonePanel;    
    public GameObject attributePanel;   
    public GameObject infoPanel;
    public GameObject BagPanel;
    public Button btnAttribute;          
    public Button btnInfo;
    public Button btnBag;
    public float showAnimTime = 0.3f;     
    public Button btnBack;
    public ScrollInventory ScrolInventory;
    
    // Notification UI
    public GameObject infoBadge;            // 未读角标容器（红点）
    public Text infoBadgeText;              // 未读数字
    public GameObject pressYHint;           // “有新消息，按Y打开手机”提示
    public AudioSource audioSource;         // 可选：音效
    public AudioClip newMessageClip;        // 可选：新消息音效

    // Chat controller (optional)
    public InfoChatController chatController; // 绑定到 InfoPanel 的聊天控制器

    [Header("Test")]
    [TextArea]
    public string testIncomingMessage = "收到一条新消息"; // 按下 M 触发
    [TextArea]
    public string testPlayerReply = "收到，我这就过去";   // 在信息栏中左键发送

    private int unreadCount = 0;
    private Coroutine pulseCoroutine = null;
    private bool isPanelVisible = false;
    private Vector2 hiddenPos;
    private Vector2 shownPos;
    private Coroutine animCoroutine;

    // 当有对方新消息后，允许玩家左键一键回复
    private bool pendingAutoReply = false;

    void Start()
    {
       
        shownPos = phonePanel.anchoredPosition;
        hiddenPos = new Vector2(shownPos.x, -phonePanel.rect.height);

        phonePanel.anchoredPosition = hiddenPos;
        phonePanel.gameObject.SetActive(false);

        attributePanel.SetActive(false);
        infoPanel.SetActive(false);
        BagPanel.SetActive(false);

        // 初始化通知UI
        if (infoBadge != null) infoBadge.SetActive(false);
        if (pressYHint != null) pressYHint.SetActive(false);
        UpdateInfoBadge();

        btnAttribute.onClick.AddListener(ShowAttributePanel);
        btnInfo.onClick.AddListener(ShowInfoPanel);
        btnBag.onClick.AddListener(ShowBagPanel);
        if (btnBack != null)
        {
            btnBack.onClick.AddListener(ReturnToHome);
            btnBack.gameObject.SetActive(false);
        }
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

        // 临时：按 M 触发一条对方消息
        if (Input.GetKeyDown(KeyCode.M))
        {
            var msg = string.IsNullOrEmpty(testIncomingMessage) ? "新消息" : testIncomingMessage;
            NotifyNewInfo(msg);
        }

        // 在信息栏内，玩家左键自动发送预设回复
        if (isPanelVisible && infoPanel.activeSelf && pendingAutoReply && Input.GetMouseButtonDown(0))
        {
            if (!string.IsNullOrEmpty(testPlayerReply))
            {
                AddPlayerMessage(testPlayerReply);
            }
            pendingAutoReply = false;
        }
    }

    void ShowPhonePanel()
    {
        phonePanel.gameObject.SetActive(true);
        isPanelVisible = true;
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PanelMoveAnim(hiddenPos, shownPos));
        Time.timeScale = 0f; // 暂停游戏
        attributePanel.SetActive(false);
        infoPanel.SetActive(false);
        BagPanel.SetActive(false);
        if (btnBack != null) btnBack.gameObject.SetActive(false);
        // 打开手机后，隐藏“按Y提示”
        if (pressYHint != null) pressYHint.SetActive(false);
    }

    void HidePhonePanel()
    {
        // 如果当前在信息栏，退出手机时清空聊天
        if (infoPanel.activeSelf && chatController != null)
        {
            chatController.ClearAll();
        }

        isPanelVisible = false;
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PanelMoveAnim(phonePanel.anchoredPosition, hiddenPos, () => phonePanel.gameObject.SetActive(false)));
        Time.timeScale = 1f; // 恢复游戏
        // 关闭手机后，如果仍有未读，可继续显示提示
        if (unreadCount > 0 && pressYHint != null) pressYHint.SetActive(true);
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
        BagPanel.SetActive(false);
        if (btnBack != null) btnBack.gameObject.SetActive(true);
    }

    void ShowInfoPanel()
    {
        attributePanel.SetActive(false);
        BagPanel.SetActive(false);
        infoPanel.SetActive(true);
        if (btnBack != null) btnBack.gameObject.SetActive(true);
        // 进入信息栏时，清除未读与引导
        ClearInfoNotifications();
    }
    void ShowBagPanel()
    {
        attributePanel.SetActive(false);
        infoPanel.SetActive(false);
        BagPanel.SetActive(true);
        ScrolInventory.Refresh();
        if (btnBack != null) btnBack.gameObject.SetActive(true);
    }

    void ReturnToHome()
    {
        attributePanel.SetActive(false);
        infoPanel.SetActive(false);
        BagPanel.SetActive(false);
        if (btnBack != null) btnBack.gameObject.SetActive(false);
    }

    // ===== 通知/聊天相关 API =====
    public void NotifyNewInfo(string message)
    {
        // 追加到聊天（对方气泡，左侧）
        if (chatController != null && !string.IsNullOrEmpty(message))
        {
            chatController.AddMessage(message, false);
        }

        // 未读 +1
        unreadCount++;
        UpdateInfoBadge();

        // 手机未打开时提示按Y
        if (!isPanelVisible && pressYHint != null)
            pressYHint.SetActive(true);

        // 高亮信息按钮
        StartInfoPulse();

        // 播放音效
        if (audioSource != null && newMessageClip != null)
            audioSource.PlayOneShot(newMessageClip);

        // 标记允许一键回复
        pendingAutoReply = true;
    }

    // 主动添加玩家消息（右侧）
    public void AddPlayerMessage(string message)
    {
        if (chatController != null && !string.IsNullOrEmpty(message))
        {
            chatController.AddMessage(message, true);
        }
    }

    void UpdateInfoBadge()
    {
        if (infoBadge != null)
            infoBadge.SetActive(unreadCount > 0);
        if (infoBadgeText != null)
            infoBadgeText.text = unreadCount > 99 ? "99+" : unreadCount.ToString();
    }

    void ClearInfoNotifications()
    {
        unreadCount = 0;
        UpdateInfoBadge();
        if (pressYHint != null) pressYHint.SetActive(false);
        StopInfoPulse();
    }

    void StartInfoPulse()
    {
        if (btnInfo == null) return;
        if (pulseCoroutine != null) return;
        pulseCoroutine = StartCoroutine(PulseButton(btnInfo.transform));
    }

    void StopInfoPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
        if (btnInfo != null)
            btnInfo.transform.localScale = Vector3.one;
    }

    IEnumerator PulseButton(Transform target)
    {
        float t = 0f;
        while (true)
        {
            t += Time.unscaledDeltaTime * 3f; // 速度
            float s = 1f + 0.06f * Mathf.Sin(t); // 轻微脉冲
            target.localScale = new Vector3(s, s, 1f);
            yield return null;
        }
    }
}