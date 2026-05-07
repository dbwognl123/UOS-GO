using System;
using UnityEngine;

[Serializable]
public class EndingSlideData
{
    public string key;

    [TextArea(2, 5)]
    public string text;

    [Header("ABAB Images")]
    public Sprite imageA;
    public Sprite imageB;

    [Header("Timing")]
    public float totalDuration = 3f;
    public float swapInterval = 0.18f;

    [Header("Motion")]
    public Vector2 startAnchoredPos = Vector2.zero;
    public Vector2 endAnchoredPos = Vector2.zero;
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = Vector3.one * 1.03f;
}