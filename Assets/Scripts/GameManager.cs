using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public bool gameStarted = false;
    public GameObject character;

    public bool paused = false;

    [SerializeField]
    public PauseMenu pauseMenu;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
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

}
