using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    public Light spotLight; // Referenz zum Spot Light
    public Light ambientLight; // Referenz auf das Ambient Light

    [Header("Normal Light Settings")]
    public float minNormalLightDuration = 2f; // Minimale Dauer f�r normales Licht
    public float maxNormalLightDuration = 5f; // Maximale Dauer f�r normales Licht

    [Header("Flicker Settings")]
    public int minFlickerCount = 8; // Minimale Anzahl von Flackermomenten
    public int maxFlickerCount = 15; // Maximale Anzahl von Flackermomenten
    public float minFlickerIntensity = 0f; // Minimale Intensit�t w�hrend des Flackerns
    public float maxFlickerIntensity = 2f; // Maximale Intensit�t w�hrend des Flackerns
    public float minFlickerInterval = 0.05f; // Minimale Dauer eines Flackermoments
    public float maxFlickerInterval = 0.2f; // Maximale Dauer eines Flackermoments

    [Header("Ambient Light Settings")]
    public float ambientIntensityMultiplier = 0.5f; // Multiplikator f�r die Ambient-Intensit�t

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
            // Normales Licht f�r eine zuf�llige Dauer
            isFlickering = false;
            spotLight.enabled = true;
            ambientLight.enabled = true;
            spotLight.intensity = maxFlickerIntensity; // Normale Lichtintensit�t
            ambientLight.intensity = maxFlickerIntensity * ambientIntensityMultiplier; // Multiplikator f�r Ambient
            float normalLightDuration = Random.Range(minNormalLightDuration, maxNormalLightDuration);
            yield return new WaitForSeconds(normalLightDuration);

            // Flackerphase mit zuf�lliger Anzahl von Flackermomenten
            isFlickering = true;
            int flickerCount = Random.Range(minFlickerCount, maxFlickerCount);
            for (int i = 0; i < flickerCount; i++)
            {
                // Zuf�llige Werte f�r das Spot Light
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
