using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This script is responsible for reading a level layout from a text file and constructing the level
 * in a Unity scene by instantiating block GameObjects. The level file should be placed in the
 * Resources folder, and each line in the file represents a row of blocks.
 *
 * WHAT YOU NEED TO DO:
 * 1. In the for loop that iterates over each character (i.e. letter) in the current row, determine
 *    which type of block to create based on the letter (e.g., use 'R' for rock, 'B' for brick, etc.).
 *
 * 2. Instantiate the correct prefab (rockPrefab, brickPrefab, questionBoxPrefab, stonePrefab) corresponding
 *    to the letter.
 *
 * 3. Calculate the position for the new block GameObject using the current row and column index.
 *    - You will likely need to maintain a separate column counter as you iterate through the characters.
 *
 * 4. Set the instantiated block’s parent to 'environmentRoot' to keep the hierarchy organized.
 *
 * ADDITIONAL NOTES:
 * - The level reloads when the player presses the 'R' key, which clears all blocks under levelRoot
 *   and then re-parses the level file.
 * - Ensure that the level file's name (without the extension) matches the 'filename' variable.
 *
 * By completing these TODOs, you will enable the level parser to dynamically create and position
 * the blocks based on the level file data.
 */

/*
 * Going to be reusing the level parser script from the platformer project to build the level from a txt file.
 * Will be creating prefabs for the enemies, barriers, and player to be placed in (hopefully) accurate positions.
 * Actually maybe I'll just do this for the enemies since barriers and the player have to be already in the level.
 * So the script can just be reading txt files for enemy positions alone.
 */

// Also as a side note, each row of the txt file when being loaded HAS TO HAVE ONE SPACE TO BE CENTERED.
// Well that's how I did it, but I think this is redundant if you have a different number of rows and columns.
// Still leaving it as a note but this fact may or may not come back to haunt me if I ever have to have a different level.

public class LevelParser : MonoBehaviour
{
    public TextAsset levelFile;
    public Transform levelRoot;

    [Header("Prefabs")]
    public GameObject enemy30Points;
    public GameObject enemy20Points;
    public GameObject enemy10Points;
    //public GameObject playerBarrier;
    //public GameObject playerTank;

    // Will have music at some point (if the og Space Invaders had any)
    // private AudioController audioController;
    
    [Header("Grid Settings")] // New variables for trying to get the grid centered and up with right spacing
    public Vector2 spacing = new Vector2(0.8f, 0.8f); // Will have to change based on scale of prefabs
    public Vector2 worldCenter = new Vector2(0.5f, 0.75f); // Be centered and about 75% up on the canvas

    
    void Start()
    {
        // Will be call in GameManager.cs once the GUI finishes its coroutine
        // LoadLevel();
        
        //if (audioController != null)
        {
            // audioController.PlayBackgroundMusicLoop(); // Play the background music
        }
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            ReloadLevel();
    }

    public void LoadLevel()
    {
        // Push lines onto a stack so we can pop bottom-up rows. This is easy to reason
        // about, but an index-based loop over the string array is faster.
        Stack<string> levelRows = new Stack<string>();

        foreach (string line in levelFile.text.Split('\n'))
            levelRows.Push(line);

        int numOfRows = levelRows.Count; // Number of rows
        int numOfColumns = 0; // Number of columns
        
        foreach (string line in levelRows)
        {
            numOfColumns = Mathf.Max(numOfColumns, line.Length);
        }
        
        
        // Getting the camera for calculations
        Camera main = Camera.main;
        float distanceForZ = -main.transform.position.z; // Currently the camera's z in the inspector is -1, make it positive for future calculations
        
        // Directly getting the center of the world from the game's display as a transform position using ViewportToWorldPoint
        Vector3 centerPoint = main.ViewportToWorldPoint(new Vector3(worldCenter.x, worldCenter.y, distanceForZ));
        // Converting the center of the screen from levelRoot's coordinates (that being the Default Level transform object that is assigned in the inspector)
        Vector3 rootCenter = levelRoot.InverseTransformPoint(centerPoint);
        
        // Needed for the startingPoint calculations as seen below
        float centerCol = (numOfColumns - 1) * 0.5f;
        float centerRow = (numOfRows - 1) * 0.5f;
        
        // Where the bottom left of the grid of prefabs should start
        Vector3 startingPoint = new Vector3(rootCenter.x - centerCol * spacing.x, rootCenter.y - centerRow * spacing.y, -1f);
        
        int row = 0;
        while (levelRows.Count > 0)
        {
            string rowString = levelRows.Pop();
            char[] rowChars = rowString.ToCharArray();
            
            for (var columnIndex = 0; columnIndex < rowChars.Length; columnIndex++)
            {
                var currentChar = rowChars[columnIndex];
                
                // Turning newPosition into a variable here so it doesn't have to be recreated everytime
                Vector3 newPosition = startingPoint + new Vector3(columnIndex * spacing.x, row * spacing.y, 0f);
                
                // Todo - Instantiate a new GameObject that matches the type specified by the character
                // Todo - Position the new GameObject at the appropriate location by using row and column
                // Todo - Parent the new GameObject under levelRoot
                
                // Three types of enemies
                if (currentChar == '3')
                {
                    Transform enemy30PointInstance = Instantiate(enemy30Points, levelRoot).transform;
                    enemy30PointInstance.localPosition = newPosition; // Had to change to localPosition instead of just position
                    enemy30PointInstance.tag = "30 Points"; // Tag this enemy with the 30 points tag
                    
                    // Needed to set the right value for animation
                    var enemy = enemy30PointInstance.GetComponent<Enemy>();
                    enemy.SetType(3);
                }
                
                if (currentChar == '2')
                {
                    Transform enemy20PointInstance = Instantiate(enemy20Points, levelRoot).transform;
                    enemy20PointInstance.localPosition = newPosition;
                    enemy20PointInstance.tag = "20 Points"; // Tag this enemy with the 20 points tag
                    
                    // Needed to set the right value for animation
                    var enemy = enemy20PointInstance.GetComponent<Enemy>();
                    enemy.SetType(2);
                }
                
                if (currentChar == '1')
                {
                    Transform enemy10PointInstance = Instantiate(enemy10Points, levelRoot).transform;
                    enemy10PointInstance.localPosition = newPosition;
                    enemy10PointInstance.tag = "10 Points"; // Tag this enemy with the 10 points tag
                    
                    // Needed to set the right value for animation
                    var enemy = enemy10PointInstance.GetComponent<Enemy>();
                    enemy.SetType(1);
                }

            }  

            row++;
        }
    }

    // --------------------------------------------------------------------------
    void ReloadLevel()
    {
        foreach (Transform child in levelRoot)
           Destroy(child.gameObject);
        
        LoadLevel();
    }
}



