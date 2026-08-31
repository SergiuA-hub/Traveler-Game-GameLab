using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private Vector3 offset;
    //Smooth camera Move
    [SerializeField] private float cameraSmooth;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Target.position + offset, Time.deltaTime* cameraSmooth);
    }
}
