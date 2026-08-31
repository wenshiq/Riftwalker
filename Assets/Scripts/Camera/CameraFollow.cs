using UnityEngine;

/// <summary>
/// 平滑跟随目标（俯视角 2D，相机固定在 z = -10）。
/// M1 用这个简单版；M3/M4 可换 Cinemachine 做屏幕震动等进阶效果。
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Transform target;
    private Vector3 velocity;

    public void SetTarget(Transform t) { target = t; }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 goal = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
    }
}
