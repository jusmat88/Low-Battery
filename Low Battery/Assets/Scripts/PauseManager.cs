using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    bool isPaused = false;

    private void OnEnable()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (isPaused) { Time.timeScale = 0; }
        else { Time.timeScale = 1; }
    }
}
