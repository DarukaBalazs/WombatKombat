using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Steps/Movement Override")]
public class MovementOverrideStep : AttackStep
{
    [SerializeField] private Vector2 overrideVelocity;

    public override void OnEnter(AttackRunner runner)
    {
        runner.Body.linearVelocity = Vector2.zero;
    }

    public override void Tick(AttackRunner runner, float time, float dt)
    {
        runner.Body.linearVelocity = overrideVelocity;
    }

    public override void OnExit(AttackRunner runner)
    {
        // opcionális: runner.Body.linearVelocity = Vector2.zero;
    }
}