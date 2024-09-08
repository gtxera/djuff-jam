using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue Objects")]
    [SerializeField] GameObject dialogueBtn;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject dialoguePanel;

    [Header("Dialogue Variables")]
    [SerializeField] string[] dialogues;
    [SerializeField] float typeDelay;
    int dialogueIndex;

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeText());
    }

    public void NextDialogue()
    {
        if (dialogueIndex + 1 == dialogues.Length) FinishDialogue();
        else
        {
            dialogueIndex++;
            dialogueBtn.SetActive(false);
            StartCoroutine(TypeText());
        }
    }

    public void FinishDialogue()
    {
        dialogueBtn.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    IEnumerator TypeText()
    {
        dialogueText.text = dialogues[dialogueIndex];
        dialogueText.maxVisibleCharacters = 0;
        for (int i = 0; i <= dialogueText.text.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }
        if (dialogueText.maxVisibleCharacters == dialogueText.text.Length) dialogueBtn.SetActive(true);
    }
}
