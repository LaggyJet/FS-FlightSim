using System;
using System.Linq;
using UnityEngine;

public class ScoreChecker : MonoBehaviour {
    static readonly float xMaxDis = 0.50859f;
    static readonly float zMaxDis = 0.68232f;
    static readonly float bestTime = 60f;
    static readonly float worstTime = 300f;
    static public float Map(float x, float in_min, float in_max, float out_min, float out_max) { return ((Math.Max(in_min, Math.Min(x, in_max)) - in_min) * (out_max - out_min) / (in_max - in_min) + out_min); }

    static public float GetHeliPadAccuracy(float xDistance, float zDistance) {  return ((Map((xDistance < 0 ? xDistance * -1 : xDistance), 0, xMaxDis, 10, 0) + Map((zDistance < 0 ? zDistance * -1 : zDistance), 0, zMaxDis, 10, 0)) / 2); }

    static public float GetOverallAccuracy(float[] accuracies) { return accuracies.Sum()/accuracies.Length; }
    
    static public float GetTimeRank(float time) { return Map(time, bestTime, worstTime, 10, 0); }
}
