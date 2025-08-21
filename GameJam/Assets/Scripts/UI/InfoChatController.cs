using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoChatController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform content;           // ScrollView/Viewport/Content
    public ScrollRect scrollRect;           // The ScrollRect controlling the list

    [Header("Prefabs")] 
    public GameObject messagePrefabLeft;    // Left bubble (other)
    public GameObject messagePrefabRight;   // Right bubble (player)

    [Header("Options")] 
    public bool autoScrollToBottom = true;

    private bool pendingScrollOnEnable = false;

    public void AddMessage(string messageText, bool isFromPlayer)
    {
        if (content == null)
        {
            Debug.LogError("InfoChatController: content is not assigned.");
            return;
        }
        var prefab = isFromPlayer ? messagePrefabRight : messagePrefabLeft;
        if (prefab == null)
        {
            Debug.LogError("InfoChatController: Missing message prefab (" + (isFromPlayer ? "Right" : "Left") + ").");
            return;
        }
        var item = Instantiate(prefab, content);
        item.transform.localScale = Vector3.one;

        // Try set text on a Text component inside the prefab
        var text = item.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            text.text = messageText;
        }
        else
        {
            Debug.LogWarning("InfoChatController: No Text found under message prefab. Message will not display.");
        }

        if (autoScrollToBottom)
        {
            if (isActiveAndEnabled)
            {
                StartCoroutine(ScrollToBottomNextFrame());
            }
            else
            {
                pendingScrollOnEnable = true;
            }
        }
    }

    private void OnEnable()
    {
        if (pendingScrollOnEnable || (autoScrollToBottom && content != null && content.childCount > 0))
        {
            pendingScrollOnEnable = false;
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    public void ClearAll()
    {
        if (content == null) return;
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        // Wait a frame so layout updates
        yield return null;
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // bottom
            Canvas.ForceUpdateCanvases();
        }
    }
}
