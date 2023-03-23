using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

//[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    #region GENERAL SETTINS
    [TabGroup("General Settings")]
    [Title("Visual Options")]

    [TabGroup("General Settings")]
    [Range(0, 25)]
    [SerializeField]
    public float spinSpeed;

    [TabGroup("General Settings")]
    public bool useEaseIn;
    [TabGroup("General Settings")][ShowIf("useEaseIn")]
    [Range(1, 5)]
    public float speedIn;
    [TabGroup("General Settings")]
    public bool useEaseOut;
    [TabGroup("General Settings")][ShowIf("useEaseOut")]
    [Range(1, 5)]
    public float speedOut;

    [TabGroup("General Settings")]
    [Title("Behaviours Options")]
    [MinMaxSlider(0, 10, true)]
    public Vector2 spinDuration;

    [TabGroup("General Settings")]
    [SerializeField]
    public float delayAmongSlots;
    #endregion

    #region RESOURCES
    [TabGroup("Resources")] [TableList] public List<GameEntries.SlotResource> resources;
    [TabGroup("Resources")] [TableList] public List<GameEntries.BlurryData> blurryData;
    #endregion

    #region FUNCTIONS

    public GameEntries.SlotResource GetRandomResource()
    {
        List<GameEntries.SlotResource> list = new();

        foreach (var slot in resources)
        {
            if (slot.type != GameEntries.SlotType.Temp)
                list.Add(slot);
        }

        return list[Random.Range(0, list.Count)];
    }
    #endregion
}