using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseWindow : MonoBehaviour
{
    private Canvas welcomePanel;

    void Start()
    {
        Time.timeScale = 0;
        welcomePanel = GetComponent<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            welcomePanel.enabled = !welcomePanel.enabled;
            Time.timeScale = (welcomePanel.enabled == true) ? 0 : 1;
        }
    }
}
