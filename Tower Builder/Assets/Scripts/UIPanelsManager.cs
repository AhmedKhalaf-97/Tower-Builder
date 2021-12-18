using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelsManager : MonoBehaviour
{
    public GameObject openPanel;
    public GameObject closePanel;

    public Animator animatorClosePanel;

    float animatorClosePanelTimeLength;

    void Awake()
    {
        animatorClosePanelTimeLength = animatorClosePanel.runtimeAnimatorController.animationClips[1].length;
    }

    public void OpenPanel()
    {
        animatorClosePanel.SetTrigger("Panel_Out");

        Invoke("InvokeOpenPanel", animatorClosePanelTimeLength);
    }

    void InvokeOpenPanel()
    {
        openPanel.SetActive(true);

        closePanel.SetActive(false);
    }
}