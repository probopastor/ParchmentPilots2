/*
* Author: Jason Marks
* ScoreManager.cs
* Stores the game score.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int stroke = 1;

    // Start is called before the first frame update
    void Start()
    {
        stroke = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore(int increaseValue)
    {
        stroke += increaseValue;
    }

    public int GetStroke()
    {
        return stroke;
    }
}
