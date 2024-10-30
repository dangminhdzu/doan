using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "Card/Create New Card")]
public class Cards : ScriptableObject
{
    public int testnum;
    public bool isUsed;
    public Sprite cardImage; // Thêm thuộc tính này để lưu trữ hình ảnh
}
