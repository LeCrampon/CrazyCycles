using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public bool gameStarted = false;
    public bool inMenu = true;
    public BikeController _player;

    public bool paused = false;

    public bool retroStyle = false;

    public PauseMenu pauseMenu;


    public static GameManager _instance { get; private set; }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void AddScore(int newScore) 
    {
        score += newScore;
        UIManager.Instance.UpdateScore(score);
    }

    public void OnPause()
    {
        if (!paused)
        {
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.gameObject.SetActive(false);
        }

        paused = !paused;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "CityGeneration")
        {
            _player = ValuesHolder._instance._bikeController;
            pauseMenu = ValuesHolder._instance._pauseMenu;
        }
    }
}
