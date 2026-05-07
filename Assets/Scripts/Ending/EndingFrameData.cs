using System;
using UnityEngine;

[Serializable]
public class EndingFrameData
{
    public Sprite image;

    [TextArea(2, 5)]
    public string narration;

    public float holdTime = 3f;
}