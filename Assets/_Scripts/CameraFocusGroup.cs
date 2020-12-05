using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusGroup : MonoBehaviour
{
    private bool testBool;

    [SerializeField] GameObject player, testdummy,cursor;
    // Start is called before the first frame update
    void Start()
    {
        TargetGroupManager.instance.AddTarget(player,10,1);
        TargetGroupManager.instance.AddTarget(cursor,5,1);
    }

    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (testBool)
            {
                TargetGroupManager.instance.AddTarget(testdummy,1,1);
            }
            else
            {
                TargetGroupManager.instance.RemoveTarget(testdummy);
            }
            testBool = !testBool;
        }
        */
    }
}
