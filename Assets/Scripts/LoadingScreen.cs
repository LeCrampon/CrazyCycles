using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{

	public TMP_Text loadingText;


	public IEnumerator AnimateLoadingText()
	{
		while (true)
		{
			loadingText.text = "Loading ";
			yield return new WaitForSeconds(.2f);
			loadingText.text = "Loading .";
			yield return new WaitForSeconds(.2f);
			loadingText.text = "Loading ..";
			yield return new WaitForSeconds(.2f);
			loadingText.text = "Loading ...";
			yield return new WaitForSeconds(.2f);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(AnimateLoadingText());
	}

}
