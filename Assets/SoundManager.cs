using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : NetworkBehaviour
{
    public AudioClip[] punch;
    public AudioClip[] kick;
    public AudioClip charge;
    public AudioClip abilityCharge;
    public AudioClip beamRelease;
    public AudioClip bomb;
    public AudioClip kiBlast;
    public AudioClip land;
    public AudioClip takeOff;
    public AudioClip death;

    AudioSource source;
    
	void Start ()
    {
        source = GetComponent<AudioSource>();
	}

    [Command]
    public void CmdMelee(string clip)
    {
        RpcMelee(clip);
    }

    [Command]
    public void CmdSoundEffect(string clip)
    {
        RpcSoundEffect(clip);
    }

    [ClientRpc]
    public void RpcMelee(string clip)
    {
        AudioClip myClip;
        switch (clip)
        {
            case "Punch":
                 myClip = punch[Random.Range(0, punch.Length)];
                if (!source.isPlaying)
                    source.PlayOneShot(myClip);
                break;

            case "Kick":
                myClip = kick[Random.Range(0, kick.Length)];
                if (!source.isPlaying)
                    source.PlayOneShot(myClip);
                break;
        }
    }

    [ClientRpc]
    public void RpcSoundEffect(string clip)
    {
        switch (clip)
        {
            case "Charge":
                if (!source.isPlaying)
                    source.PlayOneShot(charge);
                break;

            case "AbilityCharge":
                source.PlayOneShot(abilityCharge);
                break;

            case "BeamRelease":
                source.PlayOneShot(beamRelease);
                break;

            case "Bomb":
                source.PlayOneShot(bomb);
                break;

            case "KiBlast":
                if (!source.isPlaying)
                    source.PlayOneShot(kiBlast);
                break;

            case "Land":
                source.PlayOneShot(land);
                break;

            case "TakeOff":
                source.PlayOneShot(takeOff);
                break;

            case "Death":
                source.PlayOneShot(death);
                break;
        }
    }
}
