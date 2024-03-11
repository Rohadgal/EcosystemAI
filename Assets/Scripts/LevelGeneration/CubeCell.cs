using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType { Ground, Grass, Water};

public class CubeCell : MonoBehaviour
{
    //Material _material;
    MeshRenderer _meshRenderer;
    public CellType _cellType;
    [SerializeField]
    GameObject tree, grass;


    private void Awake() {
        if (_meshRenderer == null) {
            _meshRenderer = new MeshRenderer();
        }
        _meshRenderer = GetComponent<MeshRenderer>();    
        if(tree == null) {
            Debug.Log("tree prefab missing");
            return;
        }
        if (grass == null) {
            Debug.Log("grass prefab missing");
            return;
        }
        this.GetComponent<BoxCollider>().enabled = false;
        tree.GetComponent<MeshRenderer>().enabled = false;
        tree.GetComponent<CapsuleCollider>().enabled = false;
        grass.GetComponent<MeshRenderer>().enabled = false;
        grass.GetComponent <CapsuleCollider>().enabled = false;
        

    }


    public CellType getCellType() {
        return _cellType;
    }

    public void setCube(CellType cellType) {
        _cellType = cellType;
       
        // check if mesh renderer is null
        if (_meshRenderer != null) {
            switch (_cellType) {
                case CellType.Ground:
                    this.GetComponent<BoxCollider>().enabled = false;
                    Color brownColor = new Vector4(1f, .5f, .5f, 1f);
                    _meshRenderer.material.color = brownColor;
                    (tree.GetComponent<MeshRenderer>().enabled ? tree : grass).GetComponent<MeshRenderer>().enabled = false;
                    //if (tree.GetComponent<CapsuleCollider>().enabled) {
                    //    tree.GetComponent<CapsuleCollider>().enabled = false;
                    //}
                    (tree.GetComponent<CapsuleCollider>().enabled ? tree : grass).GetComponent<CapsuleCollider>().enabled = false;
                    break;
                case CellType.Grass:
                    this.GetComponent<BoxCollider>().enabled = false;
                    Color greenColor = new Vector4(.5f, .9f, 0f, 1f);
                    _meshRenderer.material.color = greenColor;
                    bool hasVegetation = UnityEngine.Random.Range(0f, 1f) < .2f;
                    if (hasVegetation) {
                        bool isTree = UnityEngine.Random.Range(0f, 1f) < .02f;
                        (isTree ? tree : grass).GetComponent<MeshRenderer>().enabled = true;
                        //if (isTree) { tree.GetComponent<CapsuleCollider>().enabled = true; }
                        (isTree ? tree : grass).GetComponent<CapsuleCollider>().enabled = true;
                    }
                    break;
                case CellType.Water:
                    Color blueColor = new Vector4(.4f, .6f, .9f, 1f);
                    _meshRenderer.material.color = blueColor;
                    (tree.GetComponent<MeshRenderer>().enabled ? tree : grass).GetComponent<MeshRenderer>().enabled = false;
                    //if (tree.GetComponent<CapsuleCollider>().enabled) {
                    //    tree.GetComponent<CapsuleCollider>().enabled = false;
                    //}
                    this.GetComponent<BoxCollider>().enabled = true;
                    (tree.GetComponent<CapsuleCollider>().enabled ? tree : grass).GetComponent<CapsuleCollider>().enabled = false;
                    break;
                default: Debug.Log("Incorrect cellType"); break;
            }
            return;
        }
        Debug.Log("Null mesh renderer");
    }
}
