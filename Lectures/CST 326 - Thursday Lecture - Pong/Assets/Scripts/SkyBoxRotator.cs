using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    public float rotationSpeed = 1f;

    public Material skyboxMaterial;

    private static readonly int RotationId = Shader.PropertyToID("_Rotation");

    void Update()
    {
        Material mat = skyboxMaterial != null ? skyboxMaterial : RenderSettings.skybox;
        if (mat == null) return;

        if (!mat.HasProperty(RotationId)) return;

        float rotation = mat.GetFloat(RotationId);
        rotation = (rotation + rotationSpeed * Time.deltaTime) % 360f;
        mat.SetFloat(RotationId, rotation);
    }
}