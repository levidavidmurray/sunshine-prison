using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueActor : InteractionActor
{

    public float dialogueTextTimeToFadeIn = 0.3f;
    public float dialogueIndicatorTimeToFadeIn = 0.15f;

    private TMP_Text dialogueText;
    private TMP_Text dialogueIndicator;
    private Transform canvas;

    private int showHideTweenId = 0;

    private int dialogueIndex = 0;

    [SerializeField]
    private string[] dialogue =
    {
        "Hello",
        "Welcome to Sunshine Prison",
        "I think you'll fit in here"
    };
    public override string InteractionText => "to talk";

    void Start() {
        canvas = transform.Find("Canvas");
        dialogueText = canvas.Find("DialogueText").GetComponent<TMP_Text>();
        dialogueIndicator = canvas.Find("DialogueIndicator").GetComponent<TMP_Text>();


        dialogueText.alpha = 0;
        dialogueIndicator.alpha = 0;
        canvas.gameObject.SetActive(false);
    }

    public override void Execute()
    {
        if (dialogueIndex == dialogue.Length)
        {
            dialogueIndex = 0;
            LeanTween.cancel(showHideTweenId);
            FadeOutDialogue();
            return;
        }

        Say(dialogue[dialogueIndex]);
        dialogueIndex++;
    }

    public void Say(string text, float displayTime = 3f) {
        canvas.gameObject.SetActive(true);
        dialogueText.text = text;

        LeanTween.cancel(showHideTweenId);

        FadeInDialogue().setOnComplete(() =>
        {
            showHideTweenId = LeanTween.delayedCall(displayTime, () =>
            {
                showHideTweenId = FadeOutDialogue().id;
            }).id;
        });
    }

    private LTDescr FadeInDialogue() {
        return LeanTween.value(0f, 1f, dialogueTextTimeToFadeIn).setOnUpdate((float val) =>
        {
            dialogueText.alpha = val;
            dialogueIndicator.alpha = val;
        });
    }

    private LTDescr FadeOutDialogue() {
        return LeanTween.value(1f, 0f, dialogueTextTimeToFadeIn).setOnUpdate((float val) =>
        {
            dialogueText.alpha = val;
            dialogueIndicator.alpha = val;
        }).setOnComplete(() =>
        {
            canvas.gameObject.SetActive(false);
        });

    }

}
