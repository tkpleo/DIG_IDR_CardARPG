using UnityEngine;

[RequireComponent(typeof(CardUI))]
public class Card : MonoBehaviour
{
    #region Fields and Properties

    [field: SerializeField] public ScriptableCard cardData { get; private set; }

    #endregion

    #region Methods

    #endregion
}
