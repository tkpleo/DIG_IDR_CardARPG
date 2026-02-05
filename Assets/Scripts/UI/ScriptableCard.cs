using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCard", menuName = "CardData")]
public class ScriptableCard : ScriptableObject
{
    [field: SerializeField] public string CardName { get; private set; }

    [field: SerializeField] public string CardDescription { get; private set; }

    [field: SerializeField] public Sprite Image { get; private set; }

    [field: SerializeField] public PlayerController player { get; private set; }
}

