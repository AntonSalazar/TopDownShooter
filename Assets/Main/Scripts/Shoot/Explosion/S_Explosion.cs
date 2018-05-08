using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class S_Explosion : S_PoolObject
{
    [Header("Explosion Settings")]
    [SerializeField] Gradient _GradientPlayer;
    [SerializeField] Gradient _GradientAI;

    ParticleSystem _ParticleSystem;
    ParticleSystem.ColorOverLifetimeModule _COLTM;
    protected override void Awake()
    {
        base.Awake();
        _ParticleSystem = GetComponent<ParticleSystem>();
        _COLTM = _ParticleSystem.colorOverLifetime;

        _UseLifeTime = true;
        _LifeTime = _ParticleSystem.main.duration;
    }

    internal void Init(bool _isPlayerOwner)
    {
        if (_isPlayerOwner) _COLTM.color = _GradientPlayer;
        else _COLTM.color = _GradientAI;
    }
}

