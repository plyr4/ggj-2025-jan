using UnityEngine;

public class BubbleMono : MonoBehaviour
{
    [SerializeField]
    public Bubble _bubble;
    [SerializeField]
    public Rigidbody _rigidbody;
    public float _mass = 4f;
    public bool _debug;

    private void Update()
    {
        if (_debug)
        {
            _rigidbody.velocity = Vector3.zero;
            _bubble._position = Vector2.one * 0.5f;
        }
    }
}