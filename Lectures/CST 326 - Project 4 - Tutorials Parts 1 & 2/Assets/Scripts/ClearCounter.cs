using UnityEngine;

// Implementing the IKitchenObjectParent interface
public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // Removed the Update portion here which solely handled the testing code as long with the two testing fields that were needed

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no kitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {
            // There is a KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
            }
            else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
