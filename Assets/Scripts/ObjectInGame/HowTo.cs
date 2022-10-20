using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowTo : IInteractable
{
    public override void interact()
    {
        string[] dialogs = { "Use [W] [A] [S] [D] to move", "Use mouse1 to fire arrow.", "Use [F] to open code panel", "Goodluck have fun." };
        DialogueController.Instance.showDialogue("HOW to play", dialogs);
    }
}
