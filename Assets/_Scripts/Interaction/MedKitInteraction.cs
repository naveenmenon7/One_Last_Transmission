
using UnityEngine;

public class MedKitInteraction : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Med Kit interaction successful!");
    }

    public string GetInteractionPrompt()
    {
        return "E  —  PICK UP MED KIT";
    }
}
