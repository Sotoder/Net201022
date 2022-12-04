using UnityEngine;

public class ShootaBoy : Boy
{
    [SerializeField] private ParticleSystem _beam;
    [SerializeField] private ParticleSystem _innerBeam;

    protected override void StartShootAnimation()
    {
        _beam.Play();
        _innerBeam.Play();
    }
}
