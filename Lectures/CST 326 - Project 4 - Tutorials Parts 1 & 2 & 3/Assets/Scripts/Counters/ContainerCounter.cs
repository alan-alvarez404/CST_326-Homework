using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // Player not carrying anything
            
            // Function call to handle spawning the kitchen object replaced the two lines that were previously here
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
        
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
