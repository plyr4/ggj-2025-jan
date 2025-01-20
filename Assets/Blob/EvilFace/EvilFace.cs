using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilFace : MonoBehaviour
{
    public Transform _face;

    void Update()
    {
        // pulsate the face scale x and y on different rates slightly
        _face.localScale = new Vector3(Mathf.Sin(Time.time) * 0.1f + 1.0f, Mathf.Cos(Time.time) * 0.1f + 1.0f, 1.0f);
    }
}