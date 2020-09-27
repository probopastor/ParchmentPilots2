/*
 * CheckpointController.cs
 * Author(s): Grant Frey
 * Created on: 9/16/2020
 * Description: Controls the checkpoint system
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    [Tooltip("The next object that the plane will rotate towards when setting up a new throw")]
    public GameObject nextCheckpoint;

    public float nextThrowHieght;

    private PlaneThrow_Handler throw_Handler;
    void Start()
    {
        throw_Handler = FindObjectOfType<PlaneThrow_Handler>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            throw_Handler.SetRotateTowardObject(nextCheckpoint);
            throw_Handler.throwHeight = nextThrowHieght;
        }
    }
}
