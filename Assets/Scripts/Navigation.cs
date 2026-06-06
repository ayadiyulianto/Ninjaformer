using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    PlayerControls controls;
    public GameObject exitPanel;
    void Awake()
    {
        controls = new PlayerControls();

        controls.UI.Cancel.performed += ctx =>
        {
            exitPanel?.SetActive(true);
        };
    }

    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
