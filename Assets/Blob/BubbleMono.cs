using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMono : MonoBehaviour
{
    [SerializeField]
    private float _delay;
    [SerializeField]
    private float _progress;
    [SerializeField]
    private float _speed;

    void Start()
    {
        _progress = 0f;
    }

    void Update()
    {
        if (_progress < 1f)
        {
            _delay -= Time.deltaTime;
            if (_delay > 0) return;
            _progress += Time.deltaTime * _speed;
            _progress = Mathf.Clamp01(_progress);
            // get material off renderer and update _Progress
            GetComponent<MeshRenderer>().material.SetFloat("_Progress", _progress);
        }
    }
}