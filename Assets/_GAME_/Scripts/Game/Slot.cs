using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class Slot : MonoBehaviour
{
    [SerializeField]  Image backgroundImg;
    [SerializeField]  Image resourceImg;

    public GameEntries.SlotData slotData;

    public void Initialize(GameEntries.SlotData slotData)
    {
        this.slotData = slotData;

        if (this.slotData.slotResource.background != null)
        {
            backgroundImg.enabled = true;
            backgroundImg.sprite = this.slotData.slotResource.background;
        }
        else
            backgroundImg.enabled = false;

        resourceImg.sprite = this.slotData.slotResource.icon;
    }

    public void SwitchResource(GameEntries.SlotResource newResource)
    {
        var index = slotData.index;
        this.slotData = new GameEntries.SlotData(newResource, index);

        if (this.slotData.slotResource.background != null)
        {
            backgroundImg.enabled = true;
            backgroundImg.sprite = this.slotData.slotResource.background;
        }
        else
            backgroundImg.enabled = false;

        resourceImg.sprite = this.slotData.slotResource.icon;
    }

}
