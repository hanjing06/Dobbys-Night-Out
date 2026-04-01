using UnityEngine;

[System.Serializable]
public class Row
{
    public bool[] row;
}

[CreateAssetMenu]
public class ArrayLayout : MonoBehaviour
{
    public Row[] rows;
}