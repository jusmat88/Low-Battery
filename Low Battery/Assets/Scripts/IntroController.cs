using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public float introLength = 3;

    private void Update()
    {
        introLength -= Time.deltaTime;

        if (introLength <= 0)
        {
            SceneManager.LoadScene(2);
        }
    }
}
