using System;
using System.Collections;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject gridElement;

    public int gridWidth { get; set; } = 80;
    public int gridDepth { get; set; } = 80;
    public int iterations = 5;

    public int numGrass = 5;
    //public int numGround = 4;
    public int numWater = 3;

    bool canGenerateCell;

    GameObject[,] cellsArray = new GameObject[0, 0];

    CellType[,] cellsArrayMap = new CellType[0, 0];

    CellType[,] cellsArrayMap2 = new CellType[0, 0];


    private void Start() {
        // Check if cell has been defined 
        if (gridElement != null) {
            generateGrid();
            return;
        }
        Debug.Log("Missing cell prefab");
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

        cellsArrayMap = new CellType[cellsArray.GetLength(0), cellsArray.GetLength(1)];


        StartCoroutine(createGrid());
    }

    public void getIterations(string input) {
        iterations = Convert.ToInt32(input);
    }

    IEnumerator createGrid() {
        
        for (int i = 0; i < gridWidth; i++) {
            for (int j = 0; j < gridDepth; j++) {
                if (canGenerateCell) {
                    setRandomCubes(i, j);
                }
            }
        }
        // Copy the array values of the random initial cubes to a bool array to know if they are alive or dead
        copyArray();

        
        for(int it=0; it<iterations; it++) {
            cellsArrayMap2 = new CellType[cellsArray.GetLength(0), cellsArray.GetLength(1)];
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridDepth; j++) {
                    // Check if the cell matrix is empty before creating a new one
                    if (canGenerateCell) {
                        //check neighbors
                        int numOfNeighbors = checkNeighbors2(i, j);
                        Debug.Log($"Tile {i},{j}  # neighbors: {numOfNeighbors}" );

                        cellsArrayMap2[i, j] = (numOfNeighbors >= numGrass) ? CellType.Grass : CellType.Water;
                    }

                }
            }
            //yield return new WaitForSeconds(2f);
            cellsArrayMap = cellsArrayMap2;
        }
   
         for (int i = 0; i < gridWidth; i++) {
             for (int j = 0; j < gridDepth; j++) {
                 // Check if the cell matrix is empty before creating a new one
                 if (canGenerateCell) {
                     //check neighbors
                     cellsArray[i, j].GetComponent<CubeCell>().setCube((cellsArrayMap[i,j] == CellType.Grass) ? CellType.Water : CellType.Grass);
                 }
             }
         }

        copyArray();

        for (int it = 0; it < 1; it++) {
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridDepth; j++) {
                    // Check if the cell matrix is empty before creating a new one
                    if (canGenerateCell) {
                        changeWaterNeighbors(i, j);
                    }
                }
            }
        }

        yield break ;
    }

    void setRandomCubes(int i, int j) {
        bool randomValue = UnityEngine.Random.value < 0.55f;
        Vector3 cubePos = new Vector3(i - gridWidth * 0.5f, 0, j - gridDepth * 0.5f);
        // Instantiate cube
        GameObject temp = Instantiate(gridElement, cubePos, Quaternion.identity);
        // set cube render to active true or false depending on random value
        temp.GetComponent<CubeCell>().setCube((randomValue) ? CellType.Water : CellType.Grass);
        temp.transform.SetParent(transform);
        // Asign cell to a position in the matrix
        cellsArray[i, j] = temp;
    }

    void changeWaterNeighbors(int x, int y) {
        if (x == 0 || y == 0 || x == gridWidth - 1 || y == gridDepth - 1) {
            return;
        }

        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (cellsArrayMap[x, y] == CellType.Water) {
                    cellsArray[x,y].transform.position = new Vector3(cellsArray[x,y].transform.position.x, -0.1f , cellsArray[x, y].transform.position.z);
                    if (cellsArrayMap[x + i, y + j] == CellType.Grass) {
                        cellsArray[x + i, y + j].GetComponent<CubeCell>().setCube(CellType.Ground);
                    }
                }
            }
        }

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
        return num;
    }

    int checkNeighbors2(int x, int y) {
        int num = 0;
        // Exclude the edges of the grid from the math
        if (x == 0 || y == 0 || x == gridWidth - 1 || y == gridDepth - 1) {
            return 0;
        }
        // Go through the neighbors of the cell and check if they are the same color as the center cell
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
               // Debug.Log($"Tile: {x},{y}   x: {x+ i}, y: {y+j}" );
                if (cellsArrayMap[x + i, y + j] == CellType.Grass) {
                    num++;
                }
            }
        }
        // Minus one to ignore the center cell comparing with itself
        return num;
    }

    // Copy matrix array to check the condition of the next generation of cells without affecting the original matrix of cells
    void copyArray() {
        //cellsArrayMap = new CellType[cellsArray.GetLength(0), cellsArray.GetLength(1)];
        //cellsArrayMap2 = new CellType[cellsArray.GetLength(0), cellsArray.GetLength(1)];
        for (int i = 0; i < cellsArray.GetLength(0); i++) {
            for (int j = 0; j < cellsArray.GetLength(1); j++) {
                cellsArrayMap[i, j] = cellsArray[i, j].GetComponent<CubeCell>().getCellType();
            }
        }
    }

    public CellType[,] getCubeMap() {
        return cellsArrayMap;
    }
}
