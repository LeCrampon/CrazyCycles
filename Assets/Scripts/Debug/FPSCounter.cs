using UnityEngine;
using System.Collections;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private float count;
    public TextMeshProUGUI text;

    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        if(count != null)
            text.text =  "FPS: " + Mathf.Round(count);
    }
}