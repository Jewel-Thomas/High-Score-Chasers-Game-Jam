using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEvent 
{
    public int points;
    public string reason;

    public ScoreEvent(int points, string reason)
    {
        this.points = points;
        this.reason = reason;
    }
}
