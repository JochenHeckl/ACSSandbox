using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftCameraClearColor : MonoBehaviour
{
    public Camera renderingCamera;

    public Color baseColor;

    public Color variant01;
    public float intervall01Sec;

    public Color variant02;
    public float intervall02Sec;

    public void Update()
    {
        var variant01Contribution = variant01 * Mathf.Max( 0f, Mathf.Cos( 2 * Mathf.PI * Time.time / intervall01Sec ) );
        var variant02Contribution = variant02 * Mathf.Max( 0f, Mathf.Sin( 2 * Mathf.PI * Time.time / intervall02Sec ) );

        renderingCamera.backgroundColor = baseColor + variant01Contribution + variant02Contribution;
    }
}
