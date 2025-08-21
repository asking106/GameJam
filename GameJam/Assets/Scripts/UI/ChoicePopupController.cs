using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChoicePopupController : MonoBehaviour
{
	[Serializable]
	public class ChoiceDefinition
	{
		public string label;
		[Header("Achievement (Optional)")]
		public bool showAchievement = false;
		[TextArea] public string achievementTitle = "成就达成！";
		[TextArea] public string achievementBody = "恭喜你完成了一个成就！";
		[Header("Events")]
		public UnityEvent onSelected;
	}

	[Header("UI References")]
	public GameObject panelRoot;            // Root panel to show/hide
	public Text titleText;                  // Optional title
	public Text bodyText;                   // Optional body/description
	public Transform choicesContainer;      // Parent to hold choice buttons
	public Button choiceButtonPrefab;       // Prefab/template for each choice (Prefab asset recommended)

	[Header("Defaults (for ShowDefault)")]
	[TextArea] public string defaultTitle;
	[TextArea] public string defaultBody;
	public List<ChoiceDefinition> choicesInInspector = new List<ChoiceDefinition>();
	[Tooltip("When showing, destroy any existing children under choicesContainer before spawning buttons.")]
	public bool clearExistingChildrenOnShow = true;

	[Header("Achievement System")]
	public AchievementPopupController achievementPopup;

	private readonly List<Button> spawnedButtons = new List<Button>();

	private void Awake()
	{
		if (panelRoot != null) panelRoot.SetActive(false);
	}

	public void ShowDefault()
	{
		// Convert inspector choices to runtime list
		var list = new List<(string label, Action onSelected)>();
		for (int i = 0; i < choicesInInspector.Count; i++)
		{
			int captured = i;
			list.Add((choicesInInspector[captured].label, () => {
				try { 
					// Show achievement if configured
					if (choicesInInspector[captured].showAchievement && achievementPopup != null)
					{
						achievementPopup.Show(choicesInInspector[captured].achievementTitle, 
							choicesInInspector[captured].achievementBody);
					}
					// Call custom events
					choicesInInspector[captured].onSelected?.Invoke(); 
				} catch (Exception e) { Debug.LogException(e); }
			}));
		}
		Show(defaultTitle, defaultBody, list);
	}

	public void Show(string title, string body, IList<(string label, Action onSelected)> choices)
	{
		if (panelRoot == null || choicesContainer == null || choiceButtonPrefab == null)
		{
			Debug.LogError("ChoicePopupController: Missing required references.");
			return;
		}

		// Set texts
		if (titleText != null) titleText.text = title ?? string.Empty;
		if (bodyText != null) bodyText.text = body ?? string.Empty;

		// Clear previous buttons and optionally any pre-existing children (e.g., template left in scene)
		ClearButtons();
		if (clearExistingChildrenOnShow)
		{
			for (int i = choicesContainer.childCount - 1; i >= 0; i--)
			{
				Destroy(choicesContainer.GetChild(i).gameObject);
			}
		}

		// Spawn new buttons
		for (int i = 0; i < choices.Count; i++)
		{
			var tuple = choices[i];
			var btn = Instantiate(choiceButtonPrefab, choicesContainer);
			spawnedButtons.Add(btn);
			var label = btn.GetComponentInChildren<Text>();
			if (label != null) label.text = tuple.label;
			btn.onClick.AddListener(() => {
				Hide();                     // 先关闭 choice 面板
				try { tuple.onSelected?.Invoke(); } catch (Exception e) { Debug.LogException(e); }
			});
		}

		panelRoot.SetActive(true);
	}

	public void Hide()
	{
		ClearButtons();
		if (panelRoot != null) panelRoot.SetActive(false);
	}

	private void ClearButtons()
	{
		for (int i = 0; i < spawnedButtons.Count; i++)
		{
			if (spawnedButtons[i] != null)
			{
				Destroy(spawnedButtons[i].gameObject);
			}
		}
		spawnedButtons.Clear();
	}
} 