using System;
using UnityEngine;

public class QuadClickHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _targetQuad;
    public LayerMask _layers;
    public bool _disable;
    private Camera _camera;
    public Action<Vector2> OnQuadClicked;

    void Update()
    {
        if (_disable) return;
        if (GameInput.Instance._firePressed)
        {
            TryHitQuad();
        }
    }

    void TryHitQuad()
    {
        if (_camera == null) _camera = Camera.main;
        if (_camera == null) return;

        Ray ray = _camera.ScreenPointToRay(GameInput.Instance._lookPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _layers))
        {
            Vector2 normalizedPosition = GetQuadNormalizedPosition(_targetQuad.transform, hit.point);
            OnQuadClicked?.Invoke(normalizedPosition);
        }
    }

    public Vector2 GetQuadNormalizedPosition(Vector3 worldHitPoint)
    {
        return GetQuadNormalizedPosition(_targetQuad.transform, worldHitPoint);
    }

    public static Vector2 GetQuadNormalizedPosition(Transform quadTransform, Vector3 worldHitPoint)
    {
        Vector3 localHitPoint = quadTransform.InverseTransformPoint(worldHitPoint);
        Vector3 scale = quadTransform.localScale;

        float xNormalized = (localHitPoint.x / scale.x) + 0.5f;
        float yNormalized = (localHitPoint.y / scale.y) + 0.5f;

        xNormalized = Mathf.Clamp01(xNormalized);
        yNormalized = Mathf.Clamp01(yNormalized);

        return new Vector2(xNormalized, yNormalized);
    }

    public Vector3 GetQuadWorldPosition(Vector2 normalizedPosition)
    {
        Vector3 scale = _targetQuad.transform.localScale;
        float x = (normalizedPosition.x - 0.5f) * scale.x;
        float y = (normalizedPosition.y - 0.5f) * scale.y;
        Vector3 p = new Vector3(x, y, 0);
        Vector3 pos = _targetQuad.transform.TransformPoint(p);
        return pos;
    }

    private void OnDrawGizmos()
    {
        if (_disable) return;
        if (_camera == null) _camera = Camera.main;
        if (_camera == null) return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 1000);
    }
}