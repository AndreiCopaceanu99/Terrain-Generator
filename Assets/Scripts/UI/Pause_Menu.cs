using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause_Menu : MonoBehaviour
{
    [SerializeField]
    GameObject Pause_Menu_UI;

    bool Active;

    // Start is called before the first frame update
    void Start()
    {
        Pause_Menu_UI.SetActive(false);
        Active = false;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Active)
            {
                Pause_Menu_UI.SetActive(false);
                Active = false;
                Time.timeScale = 1;
            }
            else
            {
                Pause_Menu_UI.SetActive(true);
                Active = true;
                Time.timeScale = 0;
            }
        }
    }

    public void Exit_To_Main_Menu()
    {
        SceneManager.LoadScene(0);
    }
}
