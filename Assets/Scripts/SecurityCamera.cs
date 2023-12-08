using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private float lookInterval = 0.1f;
    [Range(30,110)]
    [SerializeField] private float fieldOfView = 75;
    private Transform emitter;
    private GameObject player;

    private bool canSeePlayer;
    // Start is called before the first frame update
    void Start()
    {
        emitter = this.transform.GetChild(0);
        player = GameObject.Find("FirstPersonController");
        StartCoroutine(CheckForPlayer());
    }

IEnumerator CheckForPlayer()
{
    while (true)
    {
        yield return new WaitForSeconds(lookInterval);

        Ray ray = new Ray(emitter.position, player.transform.position - emitter.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                Vector3 targetDir = player.transform.position - emitter.position;
                float angle = Vector3.Angle(targetDir, emitter.forward);

                if (angle < fieldOfView)
                {
                    Debug.Log("Found the Player");
                    // Initiate Turrets
                    StartCoroutine(CallTurrets());
                    Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.green, 4);
                    canSeePlayer = true; // Set canSeePlayer to true when the player is detected
                }
                else
                {
                    Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.yellow, 4);
                    canSeePlayer = false;
                }
            }
            else
            {
                Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.red, 4);
                canSeePlayer = false;
            }
        }
    }
}
IEnumerator CallTurrets()
{
    yield return new WaitForSeconds(1);

    if (canSeePlayer)
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject turret in turrets)
        {
              TurretController turretController = turret.GetComponent<TurretController>();
    if (turretController != null)
    {
        turretController.Activate();
    }
        }
    }
}
}
