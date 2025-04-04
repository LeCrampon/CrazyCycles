using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivePopUp : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "+"+ObjectiveManager._instance._objectivePoints;

    }

    private IEnumerator DeactivatePopUp()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _audioSource.Play();
        StartCoroutine(DeactivatePopUp());
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
