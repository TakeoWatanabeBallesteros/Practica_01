using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewAudioData", menuName="Audio/AudioData")]
public class AudioData : ScriptableObject
{
    public AudioGroupType audioGroupType;
    public Sound[] sounds;
}

public enum AudioGroupType
{
    Ambient,
    Music,
    WeaponSfx,
    PlayerSfx
}
