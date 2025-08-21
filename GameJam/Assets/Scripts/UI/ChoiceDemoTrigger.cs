using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceDemoTrigger : MonoBehaviour
{
	public ChoicePopupController choicePopup;
	public AchievementPopupController achievementPopup;

	[TextArea]
	public string popupTitle;
	[TextArea]
	public string popupBody;

	private bool hasShown = false;

	private void Update()
	{
		if (!hasShown && Input.GetKeyDown(KeyCode.P))
		{
			// Prefer using inspector-driven choices
			if (choicePopup != null)
			{
				choicePopup.defaultTitle = popupTitle;
				choicePopup.defaultBody = popupBody;
				choicePopup.ShowDefault();
				hasShown = true;
			}
		}
	}
} 