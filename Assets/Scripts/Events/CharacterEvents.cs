using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CharacterEvents {
    // Character damaged and damage value
    // Unity actions have to be subscribed to directly in code (like in the UIManager and then using +=), not like Unity Events
    public static UnityAction<GameObject, int> characterDamaged;

    // Character healed and amount healed
    public static UnityAction<GameObject, int> characterHealed;
}
