using System;
using UnityEngine;

[Serializable]
public class MetaShopProductData
{
    [Header("ID")]
    public string productId;

    [Header("Category")]
    public ShopCategory category;

   

    [Header("Display")]
    public string productName;

    [TextArea(2, 5)]
    public string description;

    public Sprite productImage;

    [Header("Price")]
    public int price;

    [Header("새 게임 시작 시 능력치 보너스")]
    public StartBonusData startBonus = new StartBonusData();


    [Header("Character Ability")]
    public CharacterAbilityData characterAbility =
        new CharacterAbilityData();

}