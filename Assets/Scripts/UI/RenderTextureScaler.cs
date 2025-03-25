using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureScaler : MonoBehaviour
{
    [SerializeField]
    public RenderTexture _renderTexture;
    [SerializeField]
    public RawImage _rawImage;

    private AspectRatioFitter _fitter;

    //private void Start()
    //{
    //    _fitter = _rawImage.gameObject.AddComponent<AspectRatioFitter>();
    //    _fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
    //    _fitter.aspectRatio = (float)_renderTexture.width / _renderTexture.height;
    //}
}
