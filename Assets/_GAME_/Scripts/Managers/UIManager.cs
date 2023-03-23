using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public SpinButton m_spinButton;

    void Start()
    {
        InitGame();
    }

    void InitGame()
    {
        SetSpinButtonOn(true);
    }

    public void SetSpinButtonOn(bool isOn)
    {
        m_spinButton.SetButtonActive(isOn);
    }
 
}
