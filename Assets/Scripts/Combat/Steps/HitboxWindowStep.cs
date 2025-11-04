using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName ="Combat/Steps/Hitbox Window")]
public class HitboxWindowStep : AttackStep 
{
    [Header("Hitboxes to activate")]
    public string[] hitboxIds;

    private readonly List<Hitbox> _acitve = new();

    private void Awake()
    {

    }

}
