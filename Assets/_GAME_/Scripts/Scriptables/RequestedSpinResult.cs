using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequestedResults", menuName = "Game/RequestedResults", order = 1)]
public class RequestedSpinResult : ScriptableObject
{
    [TabGroup("Request Settings")]
    public bool randomSlots;
    [TabGroup("Request Settings")] [HideIf("randomSlots")]
    [TableList] public GameEntries.RequestedRowsData requestedRows = new GameEntries.RequestedRowsData();

    [TabGroup("SpinSettings")] 
    public bool hasSpeedUp;
    [TabGroup("SpinSettings")] [ShowIf("hasSpeedUp")]
    [Range(0, 3)]
    public int speedUpStartFrom;
}
