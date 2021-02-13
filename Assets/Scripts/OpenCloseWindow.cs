using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseWindow : MonoBehaviour
{
    private Canvas welcomePanel;

    void Start()
    {
        welcomePanel = GetComponent<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            welcomePanel.enabled = !welcomePanel.enabled;
        }
    }
}
