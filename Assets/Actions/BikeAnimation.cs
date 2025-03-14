using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeAnimation : MonoBehaviour
{

    [SerializeField]
    private Transform _spineBone;
    [SerializeField]
    private Transform _frontBone;
    [SerializeField]
    private Transform _frontWheelBone;
    [SerializeField]
    private Transform _backWheelBone;
    [SerializeField]
    private Transform _pedalierBone;
    [SerializeField]
    private Transform _leftPedaleBone;
    [SerializeField]
    private Transform _rightPedaleBone;

    [SerializeField]
    private float _turnDuration = 3f;
    private float _timer = 0;

    private Quaternion _startFrontLocalRotation;
    private Quaternion _startSpineLocalRotation;

    public BikeController bikeController;

    public bool isPlayer = true;
    private void Awake()
    {
        _startFrontLocalRotation = _frontBone.localRotation;
        _startSpineLocalRotation = _spineBone.localRotation;
    }

    public void RotateWheelBones(float rate)
    {
        Vector3 rotationAxis = new Vector3(0, -rate, 0);

        _frontWheelBone.Rotate(rotationAxis);
        _backWheelBone.Rotate(rotationAxis);
        _pedalierBone.Rotate(rotationAxis);
    }

    public void RotateFrontBone (float rate)
    {
       

        if (rate != 0 && _timer <= _turnDuration) 
        {
            Quaternion frontTurn = Quaternion.Slerp(_frontBone.localRotation, _startFrontLocalRotation * Quaternion.Euler(Vector3.up * rate * 45), _timer / _turnDuration);
            _frontBone.localRotation = frontTurn;
            _spineBone.localRotation = Quaternion.Slerp(_spineBone.localRotation, _startSpineLocalRotation * Quaternion.Euler(Vector3.down * rate * 15), _timer / _turnDuration);
            _timer += Time.deltaTime;
		}
        else if (rate ==0)
        {
            _frontBone.localRotation = Quaternion.Lerp(_frontBone.localRotation, _startFrontLocalRotation, Time.deltaTime * 2);
            _spineBone.localRotation = Quaternion.Slerp(_spineBone.localRotation, _startSpineLocalRotation, Time.deltaTime * 2);
            _timer = 0;
        }

		if (isPlayer)
		{
            if (rate > 0 && Quaternion.Angle(_frontBone.localRotation, _startFrontLocalRotation) >= 30 && bikeController.Braking)
            {
                if (bikeController.turnLeftParticles.isPlaying)
                    bikeController.turnLeftParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                if (!bikeController.turnRightParticles.isPlaying)
                    bikeController.turnRightParticles.Play();
            }
            else if (rate < 0 && Quaternion.Angle(_frontBone.localRotation, _startFrontLocalRotation) >= 30 && bikeController.Braking)
            {
                if (!bikeController.turnLeftParticles.isPlaying)
                    bikeController.turnLeftParticles.Play();
                if (bikeController.turnRightParticles.isPlaying)
                    bikeController.turnRightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                if (bikeController.turnLeftParticles.isPlaying)
                    bikeController.turnLeftParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                if (bikeController.turnRightParticles.isPlaying)
                    bikeController.turnRightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
		
	}

}
