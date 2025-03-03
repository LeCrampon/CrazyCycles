using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointPopUp : MonoBehaviour
{
    public int scoreToAdd;
    public TMP_Text text;

    private void OnEnable()
    {
        text.text = "+" + scoreToAdd;
    }

    public IEnumerator LifeCoroutine(float timeToLive = 1f)
    {
        yield return new WaitForSeconds(timeToLive);
        gameObject.SetActive(false);
    }
}
