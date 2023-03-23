using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : Singleton<GameEvents>
{
    public static GameEvent<int, GameEntries.CurrentColumnData> ColumnStopped = new();
    public static GameEvent<GameEntries.CurrentRowsData> SpinCompleted = new();

    #region Game Event Types
    public class GameEvent
    {
        event Action MyAction;

        public void AddListener(Action action)
        {
            MyAction += action;
        }

        public void RemoveListener(Action action)
        {
            MyAction -= action;
        }

        public void Invoke()
        {
            MyAction?.Invoke();
        }
    }

    public class GameEvent<T>
    {
        event Action<T> MyAction;

        public void AddListener(Action<T> action)
        {
            MyAction += action;
        }

        public void RemoveListener(Action<T> action)
        {
            MyAction -= action;
        }

        public void Invoke(T t)
        {
            MyAction?.Invoke(t);
        }
    }

    public class GameEvent<T1, T2>
    {
        event Action<T1, T2> MyAction;

        public void AddListener(Action<T1, T2> action)
        {
            MyAction += action;
        }

        public void RemoveListener(Action<T1, T2> action)
        {
            MyAction -= action;
        }

        public void Invoke(T1 t1, T2 t2)
        {
            MyAction?.Invoke(t1, t2);
        }
    }

    public class GameEvent<T1, T2, T3>
    {
        event Action<T1, T2, T3> MyAction;

        public void AddListener(Action<T1, T2, T3> action)
        {
            MyAction += action;
        }

        public void RemoveListener(Action<T1, T2, T3> action)
        {
            MyAction -= action;
        }

        public void Invoke(T1 t1, T2 t2, T3 t3)
        {
            MyAction?.Invoke(t1, t2, t3);
        }
    }
    #endregion
}
