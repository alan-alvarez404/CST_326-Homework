using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()] - Commented out to ensure a new recipe list SO cannoot be created since we only ever need one.
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOList;

}

