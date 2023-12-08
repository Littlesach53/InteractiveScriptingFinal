using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophie : MonoBehaviour
{
    public GameObject ui;
public void OnTriggerEnter(Collider other)
{
if(other.gameObject.CompareTag("Player"))
{
    ui.SetActive(true);

    StartCoroutine(Exiting());
}
}

IEnumerator Exiting()
{
    yield return new WaitForSeconds(5);

    Application.Quit();
}
}
