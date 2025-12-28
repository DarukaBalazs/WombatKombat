using System.Collections;
using UnityEngine;

public class CameraImpulseManager : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;

    Vector3 originalLocalPos;
    Coroutine shakeRoutine;

    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = transform;

        originalLocalPos = cameraTransform.localPosition;
    }

    private void OnEnable()
    {
        CameraImpulseSystem.OnCameraShake += HandleCameraShake;
    }

    private void OnDisable()
    {
        CameraImpulseSystem.OnCameraShake -= HandleCameraShake;

        if (cameraTransform != null)
            cameraTransform.localPosition = originalLocalPos;
    }

    private void HandleCameraShake(float magnitude, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(magnitude, duration));
    }

    private IEnumerator ShakeRoutine(float magnitude, float duration)
    {
        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;

            Vector2 offset = Random.insideUnitCircle * magnitude;
            cameraTransform.localPosition = originalLocalPos + (Vector3)offset;

            yield return null;
        }

        cameraTransform.localPosition = originalLocalPos;
        shakeRoutine = null;
    }
}
