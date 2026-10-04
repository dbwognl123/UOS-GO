using System;
using UnityEngine;

[Serializable]
public class CollectionEndingData
{
    [Header("ID")]
    public string endingKey;

    [Header("Display")]
    public string title;

    [TextArea(2, 5)]
    public string unlockCondition;

    [Header("Images")]
    public Sprite thumbnail;
    public Sprite detailImage;
}