using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskSelectionController : MonoBehaviour
{
    public static MaskSelectionController Instance { get; private set; } = null;

    public Mask_Input m_input;

    public int activeMaskController = 0;
    public int nextActiveMaskController;

    private bool canChangeActiveMask = true;
    public float headTurnValue = 0.5f;

    public Mask_Controller[] controllers;

    public GameObject activeMaskParent;

    public GameObject rotationParent_current;
    public GameObject rotationParent_prev;
    public GameObject rotationParent_next;

    public GameObject maskPoolParent;

    public Animator rotationAnimator;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canChangeActiveMask)
        {
            CheckHeadTurnValue();
        }
    }

    public void SetHeadTurnValue(float newValue) {
        headTurnValue = newValue;
    }

    public void CheckHeadTurnValue() {
        if(headTurnValue == 0f) {
            SelectPrevMask();
        }
        else if(headTurnValue == 1f) {
            SelectNextMask();
        }
    }

    public void SelectPrevMask()
    {
        canChangeActiveMask = false;

        nextActiveMaskController = activeMaskController - 1;
        if(nextActiveMaskController < 0) {
            nextActiveMaskController = controllers.Length-1;
        }

        controllers[activeMaskController].gameObject.transform.SetParent(rotationParent_current.transform);
        controllers[nextActiveMaskController].gameObject.transform.SetParent(rotationParent_prev.transform);

        controllers[activeMaskController].gameObject.transform.localPosition = Vector3.zero;
        controllers[activeMaskController].gameObject.transform.localRotation = Quaternion.identity;

        controllers[nextActiveMaskController].gameObject.transform.localPosition = Vector3.zero;
        controllers[nextActiveMaskController].gameObject.transform.localRotation = Quaternion.identity;


        rotationAnimator.SetTrigger("Prev");
    }

    public void SelectNextMask()
    {
        canChangeActiveMask = false;

        nextActiveMaskController = activeMaskController + 1;
        if (nextActiveMaskController > controllers.Length-1)
        {
            nextActiveMaskController = 0;
        }

        controllers[activeMaskController].gameObject.transform.SetParent(rotationParent_current.transform);
        controllers[nextActiveMaskController].gameObject.transform.SetParent(rotationParent_next.transform);

        controllers[activeMaskController].gameObject.transform.localPosition = Vector3.zero;
        controllers[activeMaskController].gameObject.transform.localRotation = Quaternion.identity;

        controllers[nextActiveMaskController].gameObject.transform.localPosition = Vector3.zero;
        controllers[nextActiveMaskController].gameObject.transform.localRotation = Quaternion.identity;

        rotationAnimator.SetTrigger("Next");
    }

    public void MaskSelectionDone()
    {
//        Debug.Log("MaskSelectionController:MaskSelectionDone");

        activeMaskController = nextActiveMaskController;
        m_input.SetActiveController(controllers[activeMaskController]);

        foreach (Mask_Controller mc in controllers) //the line the error is pointing to
        {
            mc.gameObject.transform.SetParent(maskPoolParent.transform);
        }

        controllers[activeMaskController].gameObject.transform.SetParent(activeMaskParent.transform);

        controllers[activeMaskController].gameObject.transform.localPosition = Vector3.zero;
        controllers[activeMaskController].gameObject.transform.localRotation = Quaternion.identity;

        canChangeActiveMask = true;
    }

}
