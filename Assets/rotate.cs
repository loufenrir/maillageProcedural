using UnityEngine;

public class rotate : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.LookAt(target);
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}