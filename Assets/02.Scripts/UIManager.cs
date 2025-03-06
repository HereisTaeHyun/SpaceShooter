using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 버튼 연결할 변수
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;

    void Start()
    {
        // Unity Action 이용
        action = () => OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);

        // 무명 메서드 이용, 요즘은 잘 사용되지 않음
        optionButton.onClick.AddListener(delegate {OnButtonClick(optionButton.name);});

        // 람다 이용, 주로 쓰임
        shopButton.onClick.AddListener(() => OnButtonClick(shopButton.name));
    }

    public void OnButtonClick(string msg)
    {
        Debug.Log($"메시지 클릭 발생 : {msg}");
    }
}
