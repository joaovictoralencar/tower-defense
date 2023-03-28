using UnityEngine;

[CreateAssetMenu(menuName = "Defenses/New Defense Data")]
public class DefenseData : ScriptableObject
{
    public string defenseName;
    public float defenseCost;
    public float defenseCostMultiplier = 2f;
    [TextArea]
    public string defenseDescription;
    public Tower prefab;
}


