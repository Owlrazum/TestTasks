using UnityEngine;

public class CameraAttach : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private Vector3 prevTarget;
    private void Start()
    {
        prevTarget = target.transform.position;
    }
    void Update()
    {
        Vector3 delta = target.transform.position - prevTarget;
        transform.position += delta;
        prevTarget = target.transform.position;
    }
}
