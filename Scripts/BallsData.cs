using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "Balls Data", menuName = "Scriptable Object/Balls Data", order = int.MaxValue)]

public class BallsData : ScriptableObject
{
    [Serializable]
    public struct BallData
    {
        public string name;
        public Sprite sprite;
    }
    [SerializeField]
    public BallData[] ballDatas;

}