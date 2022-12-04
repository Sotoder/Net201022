using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShootaBoy : Boy
{
    [SerializeField] private ParticleSystem _firstBeam;
    [SerializeField] private ParticleSystem _secondBeam;
    [SerializeField] private ParticleSystem _firstInnerBeam;
    [SerializeField] private ParticleSystem _secondInnerBeam;

    protected override void StartShootAnimation()
    {
        _firstBeam.Play();
        _secondBeam.Play();
        _firstInnerBeam.Play();
        _secondInnerBeam.Play();
    }
}
