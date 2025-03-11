using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _mainMenu;
	[SerializeField]
	private GameObject _controlsMenu;


	private void Awake()
	{
		
	}

	public void StartGame()
	{
		GameManager._instance.inMenu = false;
		SceneManager.LoadScene("CityGeneration");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void GoBackToMainMenu()
	{
		_controlsMenu.SetActive(false);
		_mainMenu.SetActive(true);
	}

	public void GoBackToControlsMenu()
	{
		_mainMenu.SetActive(false);
		_controlsMenu.SetActive(true);
	}

	public void SetRetroStyle(bool newRetroStyle)
    {
		GameManager._instance.retroStyle = newRetroStyle;
    }

}
