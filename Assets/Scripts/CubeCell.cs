using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType { Ground, Grass, Water};

public class CubeCell : MonoBehaviour
{
    //Material _material;
    MeshRenderer _meshRenderer;
    public CellType _cellType;


    private void Awake() {
        if (_meshRenderer == null) {
            _meshRenderer = new MeshRenderer();
        }
        _meshRenderer = GetComponent<MeshRenderer>();    
    }


    public CellType getCellType() {
        return _cellType;
    }

    public void setCube(CellType cellType) {
        _cellType = cellType;
        Color brownColor = new Vector4(1f,.5f,.5f,1f);
        // check if mesh renderer is null
        if (_meshRenderer != null) {
            switch (_cellType) {
                case CellType.Ground: _meshRenderer.material.color = brownColor;
                    break;
                case CellType.Grass: _meshRenderer.material.color = Color.green; 
                    break;
                case CellType.Water: _meshRenderer.material.color = Color.blue; 
                    break;
                default: Debug.Log("Incorrect cellType"); break;
            }
            return;
        }
        Debug.Log("Null mesh renderer");
    }
}
