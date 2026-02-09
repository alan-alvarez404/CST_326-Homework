using System.Collections;
using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    public float rotationSpeed = 1f;

    public Material skyboxMaterial;

    private static readonly int RotationId = Shader.PropertyToID("_Rotation");
    
    private float currentSpeed;
    private Coroutine slowRoutine;

    private void Awake()
    {
        currentSpeed = rotationSpeed;
    }
    
    void Update()
    {
        // Move the skybox until the game starts
        // Same as other scripts waiting to occur
        if (!GameState.CanPlay)
        {
            return;
        }
        
        Material mat = skyboxMaterial != null ? skyboxMaterial : RenderSettings.skybox;
        if (mat == null) return;

        if (!mat.HasProperty(RotationId)) return;

        float rotation = mat.GetFloat(RotationId);
        rotation = (rotation + currentSpeed * Time.deltaTime) % 360f;
        mat.SetFloat(RotationId, rotation);
    }
    
    // Will be called when a player wins
    public void slowDownSkybox(float durationSeconds)
    {
        if (slowRoutine != null)
        {
            StopCoroutine(slowRoutine);
        }

        slowRoutine = StartCoroutine(slowDownSkyboxRoutine(durationSeconds));
    }
    
    // Once again using a coroutine
    private IEnumerator slowDownSkyboxRoutine(float durationSeconds)
    {
        float startSpeed = currentSpeed;
        float t = 0f;

        while (t < durationSeconds)
        {
            t += Time.unscaledDeltaTime;
            float a = t / durationSeconds;
            currentSpeed = Mathf.Lerp(startSpeed, 0f, a);
            yield return null;
        }

        currentSpeed = 0f;
        slowRoutine = null;
    }
}