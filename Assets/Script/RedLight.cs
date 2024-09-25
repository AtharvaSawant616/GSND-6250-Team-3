using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLight : MonoBehaviour
{
    public Light myLight;
    public float maxInterval = 1f;
    public float maxFlicker = 0.2f;

    private float defaultIntensity;
    private bool isFlickering = false; // Control when to start flickering
    private bool isOn;
    private float timer;
    private float delay;

    private void Start()
    {
        defaultIntensity = myLight.intensity;
    }

    void Update()
    {
        if (!isFlickering)
        {
            // Light remains stable before flickering starts
            myLight.intensity = defaultIntensity;
            return;
        }

        // Flickering logic starts when isFlickering is true
        timer += Time.deltaTime;
        if (timer > delay)
        {
            ToggleLight();
        }
    }

    void ToggleLight()
    {
        isOn = !isOn;

        if (isOn)
        {
            // When light is "on", it returns to default intensity
            myLight.intensity = defaultIntensity;
            delay = Random.Range(0, maxInterval);
        }
        else
        {
            // Crazy flicker effect: intensity between 100 and 70
            myLight.intensity = Random.Range(70f, 100f);
            delay = Random.Range(0, maxFlicker);
        }

        timer = 0;
    }

    // Method to start flickering when key is picked up
    public void StartFlickering()
    {
        isFlickering = true; // Enable the flickering effect
    }
}