using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskRotator_Trigger : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimationEnded() {
//        Debug.Log("MaskRotator_Trigger:AnimationEnded");
        MaskSelectionController.Instance.MaskSelectionDone();
    }
}
