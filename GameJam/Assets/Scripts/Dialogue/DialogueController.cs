using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelsoftGames.PixelUI; // for UITypewriter

public class DialogueController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;                // The dialogue panel root (enable/disable)
    public UITypewriter typewriter;             // Pixel UI typewriter component
    public Transform choicesContainer;          // Parent for choice buttons
    public Button choiceButtonPrefab;           // Prefab for a single choice button
    public Text speakerNameText;                // Optional: display speaker name
    public Image speakerPortraitImage;          // Optional: display speaker portrait

    [Header("Input")]
    public KeyCode continueKey = KeyCode.E;     // Key to continue when no choices
    [SerializeField] private float inputDebounceSeconds = 0.12f; // ignore continue input right after Begin

    public event Action OnDialogueEnded;

    private DialogueData activeData = null;
    private int currentNodeIndex = -1;
    private bool isActive = false;

    private readonly List<Button> spawnedChoiceButtons = new List<Button>();

    // Cache last speaker so nodes can omit speaker fields and inherit
    private string lastSpeakerName = string.Empty;
    private Sprite lastSpeakerPortrait = null;

    private float inputUnlockedAt = 0f; // unscaled time when continue input becomes valid

    private void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void Update()
    {
        if (!isActive || activeData == null) return;

        var node = GetCurrentNode();
        if (node == null) return;

        // If there are no choices, allow pressing a key to continue (after debounce)
        if ((node.choices == null || node.choices.Count == 0)
            && Time.unscaledTime >= inputUnlockedAt
            && Input.GetKeyDown(continueKey))
        {
            GoToNode(node.nextNodeIndex);
        }
    }

    public void Begin(DialogueData data)
    {
        if (data == null || data.nodes == null || data.nodes.Count == 0)
        {
            EndConversation();
            return;
        }

        activeData = data;
        currentNodeIndex = 0;
        isActive = true;

        // reset last speaker for a fresh conversation
        lastSpeakerName = string.Empty;
        lastSpeakerPortrait = null;

        if (panelRoot != null)
            panelRoot.SetActive(true);

        // set input debounce so the starting E press doesn't skip the first node
        inputUnlockedAt = Time.unscaledTime + inputDebounceSeconds;

        ShowNode(GetCurrentNode());
    }

    private DialogueNode GetCurrentNode()
    {
        if (activeData == null) return null;
        if (currentNodeIndex < 0 || currentNodeIndex >= activeData.nodes.Count) return null;
        return activeData.nodes[currentNodeIndex];
    }

    private void ShowNode(DialogueNode node)
    {
        if (node == null)
        {
            EndConversation();
            return;
        }

        // Speaker UI with inheritance when fields are empty/null
        string nameToShow = string.IsNullOrEmpty(node.speakerName) ? lastSpeakerName : node.speakerName;
        Sprite portraitToShow = node.speakerPortrait != null ? node.speakerPortrait : lastSpeakerPortrait;

        if (speakerNameText != null)
            speakerNameText.text = string.IsNullOrEmpty(nameToShow) ? string.Empty : nameToShow;
        if (speakerPortraitImage != null)
        {
            speakerPortraitImage.sprite = portraitToShow;
            speakerPortraitImage.enabled = portraitToShow != null;
        }

        // Update cache only when provided
        if (!string.IsNullOrEmpty(node.speakerName))
            lastSpeakerName = node.speakerName;
        if (node.speakerPortrait != null)
            lastSpeakerPortrait = node.speakerPortrait;

        // Set body text with typewriter effect (with validation)
        if (typewriter == null)
        {
            Debug.LogError("DialogueController: 'typewriter' is not assigned.");
        }
        else
        {
            if (!typewriter.gameObject.scene.IsValid())
            {
                Debug.LogError("DialogueController: 'typewriter' points to a Prefab asset. Drag the instance from Hierarchy, not from Project.");
            }
            var textComp = typewriter.GetComponent<Text>();
            if (textComp == null)
            {
                Debug.LogError("DialogueController: UITypewriter must be on the same GameObject as a UnityEngine.UI.Text component.");
            }
            else
            {
                typewriter.SetText(node.text);
            }
        }

        // Render choices
        ClearChoices();
        if (node.choices != null && node.choices.Count > 0)
        {
            for (int i = 0; i < node.choices.Count; i++)
            {
                var choice = node.choices[i];
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                spawnedChoiceButtons.Add(btn);
                var btnText = btn.GetComponentInChildren<Text>();
                if (btnText != null) btnText.text = choice.text;
                int capturedIndex = i;
                btn.onClick.AddListener(() => OnChoiceClicked(capturedIndex));
            }
        }
    }

    private void OnChoiceClicked(int choiceIndex)
    {
        var node = GetCurrentNode();
        if (node == null || node.choices == null || choiceIndex < 0 || choiceIndex >= node.choices.Count) return;
        var target = node.choices[choiceIndex].nextNodeIndex;
        GoToNode(target);
    }

    private void GoToNode(int nextIndex)
    {
        if (nextIndex < 0)
        {
            EndConversation();
            return;
        }
        currentNodeIndex = nextIndex;
        ShowNode(GetCurrentNode());
    }

    private void ClearChoices()
    {
        for (int i = 0; i < spawnedChoiceButtons.Count; i++)
        {
            if (spawnedChoiceButtons[i] != null)
                Destroy(spawnedChoiceButtons[i].gameObject);
        }
        spawnedChoiceButtons.Clear();
    }

    public void EndConversation()
    {
        isActive = false;
        activeData = null;
        currentNodeIndex = -1;
        ClearChoices();
        if (panelRoot != null)
            panelRoot.SetActive(false);
        OnDialogueEnded?.Invoke();
    }
} 