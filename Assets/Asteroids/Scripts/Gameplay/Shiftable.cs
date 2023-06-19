using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiftable : MonoBehaviour
{
    /// <summary>
    /// Speed is used in translation calculation. Speed * 5 * Direction * deltaTime
    /// </summary>
    [Tooltip("Speed is used in translation calculation.\nSpeed * 5 * Direction * deltaTime")]
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected Vector3 direction;

    protected float speedModifier;

    protected bool isShifting;
    protected IEnumerator shiftCoroutine;

    void Start()
    {
        direction = direction.normalized;
        EventSystem.Singleton.OnNewGame += ProcessNewGame;

        CustomStart();
        SetShiftingCoroutine(true);
    }

    protected virtual void CustomStart()
    {
        
    }
    /// <remarks>
    /// OnEnable and OnDisable are only used for pooled objects. 
    /// UFO should not use it, because it is always active.
    /// </remarks>
    private void OnEnable()
    {
        speedModifier = speedModifier == 0 ? speed * 5 : speedModifier;
        SetShiftingCoroutine(true);
    }

    private void OnDisable()
    {
        SetShiftingCoroutine(false);
    }

    protected virtual void ProcessNewGame() 
    {
    }
    protected virtual void ModifyPosIfNeeded()
    {
        if (Settings.Singleton == null)
        {
            Debug.Log(gameObject.name);
        }
        Vector2 size = Settings.Singleton.Size;

        if (transform.position.x > size.x)
        {
            transform.position = new Vector3(0, 0, transform.position.z);
        }
        if (transform.position.x < 0)
        {
            transform.position = new Vector3(size.x, 0, transform.position.z);
        }
        if (transform.position.z < 0)
        {
            transform.position = new Vector3(transform.position.x, 0, size.y);
        }
        if (transform.position.z > size.y)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
    }

    protected virtual void SetShiftingCoroutine(bool shouldRun)
    {
        if (shouldRun && !isShifting)
        {
            shiftCoroutine = ShiftCoroutine();
            StartCoroutine(shiftCoroutine);
            isShifting = true;
        }
        else if (!shouldRun && isShifting)
        {
            StopCoroutine(shiftCoroutine);
            isShifting = false;
        }
    }

    protected virtual IEnumerator ShiftCoroutine()
    {
        while (true)
        { 
            Shift();
            ModifyPosIfNeeded();
            yield return null;
        }
    }
    protected virtual void Shift()
    {
        Vector3 translation = speedModifier * new Vector3(direction.x, 0, direction.z) * Time.deltaTime;
        transform.Translate(translation);
    }
}
