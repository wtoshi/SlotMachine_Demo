using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class GameEntries : MonoBehaviour
{
    #region Enums

    public enum BlurLevel
    {
        X2,x3
    }

    public enum SlotType
    {
        Temp, Resources1, Resources2, Resources3, Resources4, Resources5, Resources6, Resources7, Resources8
    }

    public enum GameState
    {
        Ready = 0, Spin, Stop, TurnEnd
    }

    public enum ColumnState
    {
        Idle = 0, SpinStart, SpinSpeedUp, Spin, SpinToTargetReady, SpinToTarget, SpinEnd, EaseIn, EaseOut, Stop
    }



    #endregion

    #region Game Play

    [System.Serializable]
    public class SlotData
    {
        public SlotResource slotResource;
        public int index;

        public SlotData(SlotResource slotResource, int index)
        {
            this.slotResource = slotResource;
            this.index = index;
        }
    }

    #endregion


    #region GameSettings

    [System.Serializable]
    public struct SlotResource
    {
        public SlotType type;
        [PreviewField] public Sprite background;
        [PreviewField] public Sprite icon;
    }

    [System.Serializable]
    public struct BlurryData
    {
        public BlurLevel level;
        public BlurryResource[] blurryResources;
    }

    [System.Serializable]
    public struct BlurryResource
    {
        public SlotType type;
        public Sprite resource;
    }

    [System.Serializable]
    public class CurrentRowsData
    {
        public Slot[,] currentRows = new Slot[3,5];

        public Slot GetSlot(int row, int column)
        {
            return currentRows[row, column];
        }

        public void SetColumnSlots(int columnIndex, CurrentColumnData currentColumnData)
        {
            for (int x = 0; x < currentRows.GetLength(0); x++)
            {
                currentRows[x, columnIndex] = currentColumnData.currentSlots[x];
            }
        }
    }

    [System.Serializable]
    public class CurrentColumnData
    {
        public List<Slot> currentSlots = new();

        public CurrentColumnData(Slot slot1, Slot slot2, Slot slot3)
        {
            currentSlots.Add(slot1);
            currentSlots.Add(slot2);
            currentSlots.Add(slot3);
        }
    }

    [System.Serializable]
    public class RequestedRowsData
    {
        public RowData[] desiredRows = new RowData[3];

        public RequestedRowsData(RowData firstRow, RowData secondRow, RowData thirdRow)
        {
            desiredRows[0] = firstRow;
            desiredRows[1] = secondRow;
            desiredRows[2] = thirdRow;
        }

        public RequestedRowsData()
        {
        }
    }

    [System.Serializable]
    public class RowData
    {
        public SlotType[] row = new SlotType[5];
    }

    #endregion
}
