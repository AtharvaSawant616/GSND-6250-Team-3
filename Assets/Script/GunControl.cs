 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunControl : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject FirePre;
    public Transform bulletPoint;
    public GameObject bulletPre;
    public AudioClip clip;

    public int bulletCount = 6;
    public float cd =0.2f;
    private float timer = 0;
    private AudioSource audioSource;
    public Text bulletNumber;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>cd && Input.GetMouseButtonDown(0) && bulletCount > 0){
            timer = 0;
            Instantiate(FirePre, FirePoint.position,FirePoint.rotation);
            Instantiate(bulletPre, bulletPoint.position,bulletPoint.rotation);
            bulletCount--;
            bulletNumber.text = bulletCount + "";
            audioSource.PlayOneShot(clip);
        }
        if(bulletCount == 0){
            Invoke("Reload" , 1.5f);
        }
        if(Input.GetKeyDown(KeyCode.R)){
            bulletCount = 0;
            Invoke("Reload", 1.5f);

        }
    }


    private void Reload(){
        bulletCount = 6;
        bulletNumber.text = bulletCount + "";
    }
}
