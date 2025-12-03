using System;
using UnityEngine;

public class CameraImpulseSystem : MonoBehaviour
{
    public static event Action<float, float> OnCameraShake;

    public static void TriggerShake(float magnitude, float duration)
    {
        OnCameraShake?.Invoke(magnitude, duration);
    }
}