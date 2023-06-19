using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveMobile))]
public class Skeleton : MonoBehaviour
{
    [SerializeField]
    private JoystickController joystickController;
    [SerializeField]
    private byte ID;
    [SerializeField]
    private AIBot bot; 
    [SerializeField]
    private Renderer colorRender;
    [SerializeField]
    private MoveMobile mov;
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private Transform boneStorage;
    [SerializeField]
    private float boneWidth;


    private byte boneCount;
    public byte GetID()
    {
        return ID;
    }

    public byte GetBoneCount()
    {
        return boneCount;
    }

    private Stack<Transform> bones;

    private void Awake()
    {
        bones = new Stack<Transform>();
    }
    private void Start()
    {
        ColorManager.Singleton.OnAssignColor += OnColorAssigned;
    }
    private void OnColorAssigned(byte playerID, Color color)
    {
        if (playerID != ID)
        {
            return;
        }
        UpdateColor(color);
        ColorManager.Singleton.OnAssignColor -= OnColorAssigned;
    }
    private void UpdateColor(Color color)
    {
        colorRender.material.color = color;
    }
    public bool GetBone()
    {
        if (boneCount > 0)
        {
            boneCount--;
            bones.Pop().gameObject.SetActive(false);
            mov.UpdateBonesWeight(boneCount);
            return true;
        }
        return false;
    }

    public void PickBone(Transform bone)
    {
        boneCount++;
        bones.Push(bone);
        mov.UpdateBonesWeight(boneCount);
        StartCoroutine(PickCoroutine(bone));
    }

    private IEnumerator PickCoroutine(Transform bone)
    {
        mov.SetPickingUp(true);
        joystickController?.SetActiveJoystick(false);
        bot?.SetPickingUp(true);
        yield return new WaitForSeconds(0.8f);

        bone.SetParent(rightHand, true);
        bone.localPosition = Vector3.zero;

        yield return new WaitForSeconds(1.2f);
        mov.SetPickingUp(false);
        bot?.SetPickingUp(false);
        joystickController?.SetActiveJoystick(true);

        bone.SetParent(boneStorage, true);
        bone.localPosition = Vector3.zero;
        bone.localRotation = Quaternion.Euler(0, 90, 90);
        bone.localPosition += new Vector3(0, boneCount * boneWidth, 0); 
    }
}
