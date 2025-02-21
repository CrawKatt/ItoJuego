using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public Transform objetivo;
    [SerializeField]
    float cameraVelocity;
    public Vector3 scrolling;

    private void LateUpdate()
    {
        Vector3 posicionDeseada = objetivo.position + scrolling;

        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, cameraVelocity);

        transform.position = posicionSuavizada;
    }
}
