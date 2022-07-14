using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractorGUI : MonoBehaviour
{
    [SerializeField]
    private Interactor interactor;

    [SerializeField]
    private TextMeshProUGUI objectName;
    [SerializeField]
    private TextMeshProUGUI description; 
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Canvas canvas;

    private IInteractable lastInteractable;
    private void ShowInteractable(IInteractable interactable)
    {
        if (interactable != null)
        {
            if (lastInteractable != interactable)
            {
                canvas.gameObject.SetActive(true);
                objectName.text = interactable.GetName();
                description.text = interactable.GetDescription();
                icon.sprite = interactable.GetIcon();
            }

            // Move the gui to the position of the hit.
            // Could be better to let the interactable return
            // a position instead.
            transform.position = interactor.Point;
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }

        lastInteractable = interactable;
    }

    void Update()
    {
        IInteractable interactable = interactor.GetInteractable();
        ShowInteractable(interactable);

        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
            interactor.Interact();
    }
}
