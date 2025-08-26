using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class Numbers
{
    static public float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        var ratio = (toMax - toMin) / (fromMax - fromMin);
        var c = toMin - ratio * fromMin;

        return ratio * value + c;
    }
}
