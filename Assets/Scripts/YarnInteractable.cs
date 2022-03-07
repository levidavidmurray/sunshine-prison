using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class YarnInteractable : MonoBehaviour, IPointerClickHandler
{
    public string conversationStartNode;

    private DialogueRunner m_dialogueRunner;
    private bool m_interactable = true;
    private bool m_isCurrentConversation;

    void Start() {
        m_dialogueRunner = FindObjectOfType<DialogueRunner>();
        m_dialogueRunner.onDialogueComplete.AddListener(EndConversation);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
        if (!m_interactable || m_dialogueRunner.IsDialogueRunning) return;

        StartConversation();
    }

    [YarnCommand("disable")]
    public void DisableConversation() {
        m_interactable = false;
    }

    void StartConversation() {
        m_isCurrentConversation = true;
        m_dialogueRunner.StartDialogue(conversationStartNode);
    }

    void EndConversation() {
        if (m_isCurrentConversation) m_isCurrentConversation = false;
    }
}
