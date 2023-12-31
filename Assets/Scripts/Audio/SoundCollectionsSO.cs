
using UnityEngine;

[CreateAssetMenu()]
public class SoundCollectionsSO : ScriptableObject
{
    [Header("SFX")]
    public SoundSO[] GunShoot;
    public SoundSO[] SpecialGunShoot;
    public SoundSO[] Jump;
    public SoundSO[] Splat;
    public SoundSO[] Jetpack;
    public SoundSO[] GrenadeLaunch;
    public SoundSO[] GrenadeTick;
    public SoundSO[] GrenadeExplode;
    public SoundSO[] PlayerHit;
    public SoundSO[] MegaKill;
    public SoundSO[] PowerUpPickUp;

    [Header("Music")]
    public SoundSO[] FightMusic;
    public SoundSO[] DiscoPartyMusic;
}
