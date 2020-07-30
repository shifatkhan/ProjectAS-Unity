using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnTime; // How long til you respawn

    private float respawnTimeStart;
    private bool respawn;

    private CinemachineVirtualCamera cinemachineCamera;
    private float lookaheadTime;
    private float lookaheadSmoothing;
    private float softZoneSize;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cinemachineCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        CheckRespawn();
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
    }

    private void CheckRespawn()
    {
        if(Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            player.SetActive(true);
            player.GetComponent<SpriteRenderer>().color = Color.white; // Hot fix for when player respawns with half transparency.

            // Re-position camera to player's position.
            // We use OnTargetObjectWarped to stop the lookahead from overshooting the camera.
            cinemachineCamera.OnTargetObjectWarped(player.transform, respawnPoint.position - player.transform.position);

            player.transform.position = respawnPoint.position;

            respawn = false;
        }
    }
}
