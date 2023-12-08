using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform emitter;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform player;
    [Tooltip("Inactive and Active Positions for the Turrets")]
   //  [SerializeField] private Transform inactive, active;
    private bool canSeePlayer = false;
    [SerializeField] private GameObject laserPrefab;

    private Vector3 startPostition;
    private Quaternion startRotation;

    private Queue<Rigidbody> laserPool = new Queue<Rigidbody>();

    // Start is called before the first frame update
    void Start()
    {
       anim = this.GetComponent<Animator>(); 
       startPostition = this.transform.position;
       startRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(canSeePlayer) this.transform.LookAt(player);
    }

    public void Activate()
    {
        anim.SetTrigger("Activate");
        StartCoroutine(LookForPlayer());
    }

    IEnumerator LookForPlayer()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
             Ray ray = new Ray(emitter.position, player.transform.position - emitter.position);
          RaycastHit hit;
          if(Physics.Raycast(ray, out hit, 100))
          {
            if(hit.transform.gameObject.CompareTag("Player"))
            {

            Vector3 targetDir = player.transform.position - emitter.position;
            float angle = Vector3.Angle(targetDir, emitter.forward);

            if (angle < 45) 
            {
                Debug.Log("Found the Player");
              FoundPlayer();
                Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.green, 4);
            }
            else 
            {
                Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.yellow, 4); 
                LostPlayer();
            }
            }
            else
            {
                Debug.DrawRay(emitter.position, player.transform.position - emitter.position, Color.red, 4); 
                LostPlayer();
            }
          }
        }
    }

void FoundPlayer()
{
     if(canSeePlayer == false) 
     {
        anim.SetTrigger("Firing");
        canSeePlayer = true;
     }
}

    void LostPlayer()
    {
        if(canSeePlayer)
        {
            anim.SetTrigger("Idle");
            canSeePlayer = false;
            this.transform.position = startPostition;
            this.transform.rotation = startRotation;

        }
    }

    void Shoot()
    {
        Debug.Log("Pow");
      // GameObject laser =  Instantiate(laserPrefab, emitter.position, emitter.rotation);
       Rigidbody rb;
       if(laserPool.Count > 0)
       {
        Debug.Log("Using the LaserPool");
        rb = laserPool.Dequeue();
        rb.gameObject.SetActive(true);
        rb.velocity = Vector3.zero;
       }
       else{
        rb = Instantiate(laserPrefab, emitter.position, emitter.rotation).GetComponent<Rigidbody>();
       }
       
       rb.AddRelativeForce(Vector3.forward * 100, ForceMode.Impulse);
       StartCoroutine(StoreLaser(rb));
    }

    IEnumerator StoreLaser(Rigidbody laser)
    {
        yield return new WaitForSeconds(0.4f);
        if(laser.gameObject.activeSelf == true) 
        {
       laserPool.Enqueue(laser);
        laser.gameObject.SetActive(false);
        laser.transform.position = emitter.position;
        laser.transform.rotation = emitter.rotation;
        }
 
    }
}
