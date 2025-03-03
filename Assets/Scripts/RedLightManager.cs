using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightManager : MonoBehaviour
{
    public static RedLightManager Instance { get; private set; }

    public bool firstHalfOn = true;
    public bool secondHalfOn = false;

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



    public IEnumerator SwitchRedLights()
	{
		while (GameManager.Instance.gameStarted)
		{
            yield return new WaitForSeconds(15f);

            firstHalfOn = false;
            yield return new WaitForSeconds(2f);
            secondHalfOn = true;

            yield return new WaitForSeconds(15f);

            secondHalfOn = false;
            yield return new WaitForSeconds(2f);
            firstHalfOn = true;

        }

	}
}
