using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public void GoBackToMainMenu()
    {
        Application.Quit();
    }
    
    public void EndPause()
    {
        GameManager._instance.OnPause();
    }
}
