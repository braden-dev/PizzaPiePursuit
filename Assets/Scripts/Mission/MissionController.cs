using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    private bool isInMission = false;
    private int missionId = -1;

    public void SetIsInMission(bool b, int i)
    {
        isInMission = b;
        missionId = i;
        //Debug.Log("isInMission SET: " + isInMission);
    }

    public bool GetIsInMission()
    {
        //Debug.Log("isInMission GET: " + isInMission);
        return isInMission;
    }

    public int GetMissionId()
    {
        if(isInMission)
        {
            return missionId;
        }
        return -1;
    }
}
