using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    [Header("Camera Offset")]
    public Vector3 positionOffset;

    // Update is called once per frame
    void Update()
    {
        //update the position to the the player's plus the offset
        transform.position = player.transform.position + positionOffset;

        //have the camera look at the player
        transform.LookAt(player.transform.position);
    }
}
