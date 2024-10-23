using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedKit : MonoBehaviour
{
    public Text medNum;
    public int medCount;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            medCount++;
            medNum.text = medCount.ToString();
            Destroy(gameObject);

        }
    }
}
