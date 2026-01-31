using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParamByTime : SingletonMonoBehaviour<ParamByTime>
{
    [SerializeField] private TimerParamSet _timerParamSet;

    public float GetMoveDuration(float callTime)
    {
        if (GetTimerParamBase(callTime, out var timerParamBase) == false)
        {
            return 1f;
        }
        return timerParamBase.MoveDuration;
    }

    public float GetWaitDuration(float callTime)
    {
        if (GetTimerParamBase(callTime, out var timerParamBase) == false)
        {
            return 1f;
        }
        return timerParamBase.WaitDuration;
    }

    public float GetSpawnInterval(float callTime)
    {
        if (GetTimerParamBase(callTime,out var timerParamBase)==false)
        {
            return 1f;
        }
        return timerParamBase.SpawnInterval;
    }
    public bool GetTimerParamBase(float callTime, out TimerParamBase timerParamBase)
    {
        List<TimerParamBase> targetParam = _timerParamSet.TimerParams.FindAll(param => callTime >= param.CallTime);
        if (targetParam == null || targetParam.Count == 0)
        {
            timerParamBase = null;
            return false;
        }
        timerParamBase = targetParam
            .OrderByDescending(p => p.CallTime)
            .FirstOrDefault();
        return timerParamBase != null;
    }
}
