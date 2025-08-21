using UnityEngine;

public class NPCdialog : MonoBehaviour
{
    private bool isin = false;
    private Playermovement playermove;

    // Data-driven dialogue
    public DialogueData dialogueData;
    public DialogueController dialogueController;
    private bool hasStartedDialogue = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isin)
        {
            if (dialogueData != null && dialogueController != null && !hasStartedDialogue)
            {
                hasStartedDialogue = true;
                if (playermove != null) playermove.enabled = false;
                dialogueController.OnDialogueEnded += HandleDialogueEnded;
                dialogueController.Begin(dialogueData);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playermove = collision.GetComponent<Playermovement>();
            isin = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isin = false;
        }
    }

    private void HandleDialogueEnded()
    {
        if (playermove != null) playermove.enabled = true;
        hasStartedDialogue = false;
        if (dialogueController != null) dialogueController.OnDialogueEnded -= HandleDialogueEnded;
    }
}
