using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    private bool isInMission = false;

    public void SetIsInMission(bool b)
    {
        isInMission = b;
        //Debug.Log("isInMission SET: " + isInMission);
    }

    public bool GetIsInMission()
    {
        //Debug.Log("isInMission GET: " + isInMission);
        return isInMission;
    }
}
