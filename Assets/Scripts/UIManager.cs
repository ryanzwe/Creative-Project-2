using System;
using UnityEngine.UI;
using UnityEngine;

[Serializable]
public class UIManager
{
    [Header("Weapon")]
    public Text AmmoCount;
    public Text ClipCount;

    [Header("Player")]
    public Slider Health;

    public Text Score;

    [Header("Gameplay Directions")]
    public Text StickCount;

    public Image StickImage;
}
