using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopupController : MonoBehaviour
{
	[Header("UI References")]
	public GameObject panelRoot;
	public Text titleText;
	public Text bodyText;
	public float showSeconds = 2.5f; // how long to stay visible
	public bool pauseGameWhileShowing = true;

	private bool isShowing = false;

	private void Awake()
	{
		if (panelRoot != null) panelRoot.SetActive(false);
	}

	public void Show(string title, string body)
	{
		if (isShowing) return;
		isShowing = true;
		if (titleText != null) titleText.text = title ?? string.Empty;
		if (bodyText != null) bodyText.text = body ?? string.Empty;
		if (panelRoot != null) panelRoot.SetActive(true);
		if (pauseGameWhileShowing) Time.timeScale = 0f;
		StartCoroutine(AutoHide());
	}

	private IEnumerator AutoHide()
	{
		float endAt = Time.unscaledTime + showSeconds;
		while (Time.unscaledTime < endAt)
		{
			yield return null;
		}
		Hide();
	}

	public void Hide()
	{
		if (pauseGameWhileShowing) Time.timeScale = 1f;
		if (panelRoot != null) panelRoot.SetActive(false);
		isShowing = false;
	}
} 