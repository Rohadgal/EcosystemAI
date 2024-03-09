using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    None,
    LoadMenu,
    ChangeLevel,
    Playing,
    GameFinished
}

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;

    public int numberOfLions { get; private set; }
    public int numberOfDogs { get; private set; }
    public int numberOfCats { get; private set; }
    public int numberOfChickens { get; private set; }

    private GameState m_gameState;
    int levelIndex;

    // Awake is called when the script instance is being loaded
    private void Awake() {
        // Singleton pattern to ensure there's only one instance of GameManager
        if (s_instance != null && s_instance != this) {
            Destroy(this);
        } else {
            s_instance = this;
        }

        m_gameState = GameState.None;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Setters
    public void SetNumberOfLions(int number) {
        numberOfLions = number;
    }
    public void SetNumberOfDogs(int number) {
        numberOfDogs = number;
    }
    public void SetNumberOfCats(int number) {
        numberOfCats = number;
    }
    public void SetNumberOfChickens(int number) {
        numberOfChickens = number;
    }
    #endregion
}
