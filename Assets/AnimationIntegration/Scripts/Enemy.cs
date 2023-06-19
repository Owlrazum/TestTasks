using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Used only for triggering
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform back;
    [SerializeField]
    private Animator anim;

    Rigidbody[] rigidbodies;
    Collider coll;

    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        coll = GetComponent<Collider>();
        TurnOffRagDoll();
    }

    public Transform GetBackTransform()
    {
        return back;
    }

    public void SetFinishing()
    {
        coll.enabled = false;
        StartCoroutine(TurnOnRagDoll());
    }

    private IEnumerator TurnOnRagDoll()
    {
        yield return new WaitForSeconds(0.75f);
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        anim.enabled = false;
        yield return new WaitForSeconds(5);
        transform.position = new Vector3(Random.Range(-15f, 15f), 0, Random.Range(-5f, 15f));
        transform.rotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);
        TurnOffRagDoll();
    }

    private void TurnOffRagDoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        anim.enabled = true;
        coll.enabled = true;
    }
}
