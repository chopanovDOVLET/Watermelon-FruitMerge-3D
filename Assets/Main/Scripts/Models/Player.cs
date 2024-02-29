using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Player
{
    public long Score;
    public long HighScore;

    public Player(long score, long highScore)
    {
        Score = score;
        HighScore = highScore;
    }
}
