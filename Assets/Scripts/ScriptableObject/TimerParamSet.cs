using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimerParamSet", menuName = "ScriptableObject/TimerParamSet", order = 0)]
public class TimerParamSet : ScriptableObject
{
   public List<TimerParamBase> TimerParams;
}
[Serializable]
public class TimerParamBase
{
    public float CallTime;
    public float MoveDuration;
    public float WaitDuration;
    public float SpawnInterval;
}
