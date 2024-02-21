using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayForOtherScene : MonoBehaviour
{
    public void SFX(string _name)
    {
        SoundManager.Instance.PlaySFXSound(_name);
    }

    public void BGM(string _name)
    {
        SoundManager.Instance.PlayBGMSound(_name);
    }
}
