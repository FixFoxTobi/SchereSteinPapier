using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    public Light spotLight; // Referenz zum Spot Light
    public Light ambientLight; // Referenz auf das Ambient Light

    [Header("Normal Light Settings")]
    public float minNormalLightDuration = 2f; // Minimale Dauer für normales Licht
    public float maxNormalLightDuration = 5f; // Maximale Dauer für normales Licht

    [Header("Flicker Settings")]
    public int minFlickerCount = 8; // Minimale Anzahl von Flackermomenten
    public int maxFlickerCount = 15; // Maximale Anzahl von Flackermomenten
    public float minFlickerIntensity = 0f; // Minimale Intensität während des Flackerns
    public float maxFlickerIntensity = 2f; // Maximale Intensität während des Flackerns
    public float minFlickerInterval = 0.05f; // Minimale Dauer eines Flackermoments
    public float maxFlickerInterval = 0.2f; // Maximale Dauer eines Flackermoments

    [Header("Ambient Light Settings")]
    public float ambientIntensityMultiplier = 0.5f; // Multiplikator für die Ambient-Intensität

    private bool isFlickering = false;

    private void Start()
    {
        if (spotLight == null)
        {
            spotLight = GetComponent<Light>();
        }
        if (ambientLight == null)
        {
            ambientLight = GetComponent<Light>();
        }

        // Startet die Flacker-Routine
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Normales Licht für eine zufällige Dauer
            isFlickering = false;
            spotLight.enabled = true;
            ambientLight.enabled = true;
            spotLight.intensity = maxFlickerIntensity; // Normale Lichtintensität
            ambientLight.intensity = maxFlickerIntensity * ambientIntensityMultiplier; // Multiplikator für Ambient
            float normalLightDuration = Random.Range(minNormalLightDuration, maxNormalLightDuration);
            yield return new WaitForSeconds(normalLightDuration);

            // Flackerphase mit zufälliger Anzahl von Flackermomenten
            isFlickering = true;
            int flickerCount = Random.Range(minFlickerCount, maxFlickerCount);
            for (int i = 0; i < flickerCount; i++)
            {
                // Zufällige Werte für das Spot Light
                float randomSpotIntensity = Random.Range(minFlickerIntensity, maxFlickerIntensity);
                bool randomEnabled = Random.value > 0.5f;
                float flickerInterval = Random.Range(minFlickerInterval, maxFlickerInterval);

                // Spot Light einstellen
                spotLight.intensity = randomSpotIntensity;
                spotLight.enabled = randomEnabled;

                // Ambient Light basierend auf dem Spot Light einstellen
                ambientLight.intensity = randomSpotIntensity * ambientIntensityMultiplier;
                ambientLight.enabled = randomEnabled;

                yield return new WaitForSeconds(flickerInterval);
            }

            // Licht wieder aktivieren
            spotLight.enabled = true;
            ambientLight.enabled = true;
        }
    }
}
