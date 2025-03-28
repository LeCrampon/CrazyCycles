using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraSmoothFollow : MonoBehaviour
{

    public float smoothPosFactor = 1f;
    public float smoothRotFactor = 1.5f;

    [SerializeField]
    private float _heightOffset;
    [SerializeField]
    private float _distanceOffset;
    [SerializeField]
    private Transform _target;

    public BikeController _playerController;


    [SerializeField]
    private float _heightOffsetRagdoll;
    [SerializeField]
    private float _distanceOffsetRagdoll;


    private Vector3 velocity = Vector3.zero;

    [SerializeField]
    public RenderTexture retroTexture;

    [SerializeField]
    public Canvas renderUI;

    [SerializeField]
    public Canvas mainUI;

    private void Awake()
    {
        SetRenderTexture();
    }
    private void Start()
    {
        transform.position = _target.position - _target.forward * _distanceOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y + _heightOffset, transform.position.z);
        transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
    }

    void FixedUpdate()
    {
        if (!_playerController.isRagdoll)
        {
            Vector3 newPos = _target.position - _target.forward * _distanceOffset;
            newPos = new Vector3(newPos.x, newPos.y + _heightOffset, newPos.z);
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothPosFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.position - transform.position), smoothRotFactor * Time.fixedDeltaTime);
        }
        else
        {
            ZoomBack();
        }
   
        
    }

    private void ZoomBack()
    {

       transform.position -= transform.forward/8;
    }

    private void SetRenderTexture()
    {
        if (GameManager._instance.retroStyle)
        {
            Camera.main.targetTexture = retroTexture;
            renderUI.gameObject.SetActive(true);
            mainUI.scaleFactor = 1;
        }
        else
        {
            Camera.main.targetTexture = null;
            renderUI.gameObject.SetActive(false);
            mainUI.scaleFactor = 4;
        }
    }
}
