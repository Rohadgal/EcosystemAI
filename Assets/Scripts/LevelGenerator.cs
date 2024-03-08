using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject gridElement;

    int gridWidth = 0;
    int gridDepth = 0;
    int iterations = 1;

    int numGrass = 3;
    int numGround = 5;
    int numWater = 8;

    float prefabWidth;
    float prefabDepth;

    bool canGenerateCell;
    Vector3 topLeftCorner;

    bool m_isStepped = true;

    GameObject[,] cellsArray = new GameObject[0, 0];

    //GameObject[,] cellsArrayOne = new GameObject[0, 0];
    CellType[,] cellsArrayMap = new CellType[0, 0];

    private void Awake() {
        UpdatePosition();
    }

    private void Start() {
        // Check if cell has been defined and check and assign it's dimension
        if (gridElement != null) {
            //prefabWidth = gridElement.GetComponent<MeshRenderer>().bounds.size.x;
            //prefabDepth = gridElement.GetComponent<MeshRenderer>().bounds.size.z;
            generateGrid();
            return;
        }
        Debug.Log("Missing cell prefab");
    }

    void Update() {
        // Check if the screen size has changed and update the position
        if (Screen.width != transform.position.x || Screen.height != transform.position.y) {
            UpdatePosition();
        }
    }

    public void generateGrid() {
        
        if (canGenerateCell) {
            canGenerateCell = false;
            StopAllCoroutines();
        }

        // Check if matrix array size has not been defined and set a default size if it hasn´t.
        if (cellsArray.Length == 0) {
            if (gridWidth == 0 || gridDepth == 0) {
                gridWidth = 30;
                gridDepth = 30;
            }
            cellsArray = new GameObject[gridDepth, gridWidth];
        }

        if (gridWidth == 0) {
            gridWidth = 50;
        }
        if (gridDepth == 0) {
            gridDepth = 50;
        }
        canGenerateCell = true;
      
        StartCoroutine(createGrid());
    }

    // Assign grid width
    public void getWidthInput(string input) {
        gridWidth = Convert.ToInt32(input);
    }
    // Assign grid height
    public void getHeightInput(string input) {
        gridDepth = Convert.ToInt32(input);
    }
    //// Assign number of similar cells for alive ones to continue living
    //public void getAliveNum(string input) {
    //    numAlive = Convert.ToInt32(input);
    //}
    //// Assign number of similar cells for dead ones to continue dead
    //public void getDeadNum(string input) {
    //    numDead = Convert.ToInt32(input);
    //}

    public void getIterations(string input) {
        iterations = Convert.ToInt32(input);
    }

    public void isStepped(bool input) {
        m_isStepped = input;
    }

    // Set the position to the top-left corner of the screen
    void UpdatePosition() {
        topLeftCorner = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height*0.25f, 0));
    }

    IEnumerator createGrid() {
        
        for (int i = 0; i < gridWidth; i++) {
            for (int j = 0; j < gridDepth; j++) {
                if (canGenerateCell) {
                    setRandomCubes(i, j);
                    // Create a little pause before drawing each individual cell on the matrix
                    //if (m_isStepped) {
                    //    yield return new WaitForSeconds(0.02f);
                    //}
                }
            }
        }
        // Copy the array values of the random initial cubes to a bool array to know if they are alive or dead
        copyArray();

        for(int it=0; it<iterations; it++) {
           // yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridDepth; j++) {
                    // Check if the cell matrix is empty before creating a new one
                    if (canGenerateCell) {
                        //check neighbors

                        int numOfNeighbors = checkNeighbors(i, j);

                        switch (cellsArrayMap[i, j]) {
                            case CellType.Grass:
                                if(numOfNeighbors == numWater) {
                                    cellsArray[i, j].GetComponent<CubeCell>().setCube(CellType.Water); break;
                                }
                                cellsArray[i, j].GetComponent<CubeCell>().setCube((numOfNeighbors >= numGrass) ? CellType.Grass : CellType.Ground);
                                break;
                            case CellType.Ground:
                                cellsArray[i, j].GetComponent<CubeCell>().setCube((numOfNeighbors >= numGround) ? CellType.Ground : CellType.Grass);
                                break;
                            default: Debug.Log("Error with cell type"); break;
                        }
                        //if (m_isStepped) {
                        //    yield return new WaitForSeconds(0.02f);
                        //}
                    }

                }
            }   
        }
        yield break ;
    }

    void setRandomCubes(int i, int j) {
       // Debug.Log("This");
        bool randomValue = UnityEngine.Random.Range(0, 100) < 30;
        Vector3 cubePos = new Vector3(i - gridWidth * 0.5f, 0, j - gridDepth * 0.5f);
        // Instantiate cube
        GameObject temp = Instantiate(gridElement, cubePos, Quaternion.identity);
    
        // set cube render to active true or false depending on random value
        temp.GetComponent<CubeCell>().setCube((randomValue) ? CellType.Ground : CellType.Grass);
       
        temp.transform.SetParent(transform);
        // Asign cell to a position in the matrix
        cellsArray[i, j] = temp;
    }

    int checkNeighbors(int x, int y) {
        int num = 0;
        // Exclude the edges of the grid from the math
        if (x == 0 || y == 0 || x == gridWidth - 1 || y == gridDepth - 1) {
            return 0;
        }
        // Go through the neighbors of the cell and check if they are the same color as the center cell
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (cellsArrayMap[x + i, y + j] == cellsArrayMap[x, y]) {
                    num++;
                }
            }
        }
        // Minus one to ignore the center cell comparing with itself
        return num - 1;
    }

    // Copy matrix array to check the condition of the next generation of cells without affecting the original matrix of cells
    void copyArray() {
        cellsArrayMap = new CellType[cellsArray.GetLength(0), cellsArray.GetLength(1)];
        for (int i = 0; i < cellsArray.GetLength(0); i++) {
            for (int j = 0; j < cellsArray.GetLength(1); j++) {
                cellsArrayMap[i, j] = cellsArray[i, j].GetComponent<CubeCell>().getCellType();
            }
        }
    }
}
