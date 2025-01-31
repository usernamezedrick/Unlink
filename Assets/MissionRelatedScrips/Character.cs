using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    // Events for actions the character takes
    public event Action OnEnemyDefeated;
    public event Action OnModuleInteracted;
    public event Action OnClueInteracted;

    // These methods are called when the character performs actions like defeating an enemy or interacting with objects
    public void DefeatEnemy()
    {
        Debug.Log("Enemy defeated!");
        OnEnemyDefeated?.Invoke();  // Trigger the event
    }

    public void InteractWithModule()
    {
        Debug.Log("Module interacted with!");
        OnModuleInteracted?.Invoke();  // Trigger the event
    }

    public void InteractWithClue()
    {
        Debug.Log("Clue interacted with!");
        OnClueInteracted?.Invoke();  // Trigger the event
    }

    // You can call these methods from the relevant scripts, e.g., when an enemy is defeated or a module/clue is interacted with
}
