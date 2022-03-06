using UnityEngine;
using TMPro;

[RequireComponent(typeof(SphereCollider))]
public class InteractableActor : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private Canvas canvas;

    private Transform _interactionPromptXbox;
    private Transform _interactionPromptPC;
    private Transform _interactionPromptPS5;

    [SerializeField]
    private InteractionActor _interactionActor;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        canvas.gameObject.SetActive(false);

        _interactionPromptXbox = canvas.transform.Find("InteractionPrompt_Xbox");
        _interactionPromptPC = canvas.transform.Find("InteractionPrompt_PC");
        _interactionPromptPS5 = canvas.transform.Find("InteractionPrompt_PS5");

        if (!_interactionActor) return;

        SetInteractionText(_interactionPromptPC, _interactionActor.InteractionText);
        SetInteractionText(_interactionPromptXbox, _interactionActor.InteractionText);
        SetInteractionText(_interactionPromptPS5, _interactionActor.InteractionText);
    }

    public bool IsActorType(System.Type type)
    {
        return type == _interactionActor.GetType();
    }

    public void ShowPromptInstructions(ControlScheme controlScheme)
    {
        SetActiveControlsPrompt(controlScheme);
        canvas.gameObject.SetActive(true);
    }

    public void HidePromptInstructions()
    {
        canvas.gameObject.SetActive(false);
    }

    public void Interact()
    {
        HidePromptInstructions();
        _interactionActor.Execute();
    }

    private void SetActiveControlsPrompt(ControlScheme controlScheme)
    {
        switch (controlScheme)
        {
            case ControlScheme.Gamepad:
                _interactionPromptPS5.gameObject.SetActive(false);
                _interactionPromptPC.gameObject.SetActive(false);
                _interactionPromptXbox.gameObject.SetActive(true);
                break;
            case ControlScheme.KeyboardMouse:
                _interactionPromptPS5.gameObject.SetActive(false);
                _interactionPromptXbox.gameObject.SetActive(false);
                _interactionPromptPC.gameObject.SetActive(true);
                break;
        }
    }

    private void SetInteractionText(Transform interactionPrompt, string text)
    {
        interactionPrompt.Find("InteractionText").GetComponent<TMP_Text>().text = text;
    }

}
