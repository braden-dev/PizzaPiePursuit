using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    private bool isInMission = false;
    private int missionId = -1;

    private static bool mission1Complete = false;
    private static bool mission2Complete = false;
    private static bool mission3Complete = false;
    private static bool mission4Complete = false;

    private static float mission1BestTime = 1000.0f;
    private static float mission2BestTime = 1000.0f;
    private static float mission3BestTime = 1000.0f;
    private static float mission4BestTime = 1000.0f;

    public TextMeshProUGUI activeMissionDisplayText;

    private void Update()
    {
        if (missionId == -1)
            activeMissionDisplayText.text = "No Delivery Active";
    }

    public void SetIsInMission(bool b, int i)
    {
        isInMission = b;
        missionId = i;
        activeMissionDisplayText.text = "Delivery #" + i;
    }

    public bool GetIsInMission()
    {
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

    public void SetMissionComplete(int missionId, float missionCompletionTime)
    {
        switch (missionId)
        {
            case (1):
                mission1Complete = true;
                if(missionCompletionTime < mission1BestTime)
                {
                    mission1BestTime = missionCompletionTime;
                    //Debug.Log("New best time: " + mission1BestTime);
                }
                break;
            case (2):
                mission2Complete = true;
                if (missionCompletionTime < mission2BestTime)
                {
                    mission2BestTime = missionCompletionTime;
                }
                break;
            case (3):
                mission3Complete = true;
                if (missionCompletionTime < mission3BestTime)
                {
                    mission3BestTime = missionCompletionTime;
                }
                break;
            case (4):
                mission4Complete = true;
                if (missionCompletionTime < mission4BestTime)
                {
                    mission4BestTime = missionCompletionTime;
                }
                break;
            default:
                break;
        }
    }

    public bool GetMission1Complete()
    {
        return mission1Complete;
    }

    public float GetMission1BestTime()
    {
        return mission1BestTime;
    }

    public bool GetMission2Complete()
    {
        return mission2Complete;
    }

    public float GetMission2BestTime()
    {
        return mission2BestTime;
    }

    public bool GetMission3Complete()
    {
        return mission3Complete;
    }

    public float GetMission3BestTime()
    {
        return mission3BestTime;
    }

    public bool GetMission4Complete()
    {
        return mission4Complete;
    }

    public float GetMission4BestTime()
    {
        return mission4BestTime;
    }
}
