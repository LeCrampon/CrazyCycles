using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField]
    private Light _light;

    [SerializeField]
    private float _maxRange;
    [SerializeField]
    private float _minRange;
    [SerializeField]
    private float _rangeSpeed;
    [SerializeField]
    private float _intensitySpeed;

    private bool _reducingRange = false;


    void Update()
    {
        if (!_reducingRange)
        {
            if (_light.range < _maxRange)
            {
                _light.range += Time.deltaTime * _rangeSpeed;
                _light.intensity += Time.deltaTime * _intensitySpeed;
            }
            else if (_light.range >= _maxRange)
            {
                _reducingRange = true;
            }
        }
        else
        {
            if (_light.range > _minRange)
            {
                _light.range -= Time.deltaTime * _rangeSpeed;
                _light.intensity -= Time.deltaTime * _intensitySpeed;
            }
            else if (_light.range <= _minRange)
            {
                _reducingRange = false;
            }
        }
       
    }

}
