using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Castles Data", menuName = "Scriptable Object/Castles Data", order = int.MaxValue)]

public class CastlesData : ScriptableObject
{
    [Serializable]
    public class CastleData
    {
        public string name;
        public Sprite sprite;
    }
    [SerializeField]
    public CastleData[] castleDatas;

}