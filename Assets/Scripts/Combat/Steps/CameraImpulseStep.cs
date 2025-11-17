using UnityEngine;
using Combat;

[CreateAssetMenu(menuName = "Combat/Steps/Camera Impulse")]
public class CameraImpulseStep : AttackStep
{
    [SerializeField] private Transform camPos;
    [SerializeField] private float strength;

    private bool shaking = false;
    private Vector3 originalPos;

    public override void OnEnter(AttackRunner ctx)
    {
        if (camPos != null)
            originalPos = camPos.localPosition;

        shaking = true;
    }

    public override void Tick(AttackRunner ctx, float localTime, float dt)
    {
        if (!shaking || camPos == null)
            return;

        camPos.localPosition = originalPos + Random.insideUnitSphere * strength;
    }

    public override void OnExit(AttackRunner ctx)
    {
        shaking = false;

        if (camPos != null)
            camPos.localPosition = originalPos;
    }
}