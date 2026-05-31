using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public string Scene = "Main";
    public Button Retry;

    public void Start()
    {
        Retry.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(Scene);
            }
        );
    }
}
