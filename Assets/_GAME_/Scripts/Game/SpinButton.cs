using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
    [SerializeField] Button spinButton;

    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        GameManager.Instance.StartGame();
    }

    public void SetButtonActive(bool _mode)
    {
        spinButton.interactable = _mode;
    }

}