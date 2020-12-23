using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {   
        button.onClick.AddListener(TaskOnClick);
    }
    
    public void TaskOnClick()
    {
        Debug.Log("You have clicked the " + name + " button!");

        if(name == "StartButton")
        {
            Scene scene = SceneManager.LoadScene("testKitchen", new LoadSceneParameters(LoadSceneMode.Single));
        }

        else if(name == "QuitButton")
        {
            Application.Quit();
        }
        else if(name == "SettingsButton")
        {
            Scene scene = SceneManager.LoadScene("Settings", new LoadSceneParameters(LoadSceneMode.Single));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
