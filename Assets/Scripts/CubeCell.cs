using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType { Ground, Grass, Water};

public class CubeCell : MonoBehaviour
{
    [SerializeField]
    bool _isAlive = true;
    public bool _isWhite = true;
    Material _material;
    MeshRenderer _meshRenderer;
    public CellType _cellType;
    //SpriteRenderer _spriteRenderer;

    public int _state;

    private void Awake() {
        if (_meshRenderer == null) {
            _meshRenderer = new MeshRenderer();
        }
        if (_material == null) {
            _material = new Material(Shader.Find("Standard"));
            _material.color = new Color(1, 1, 1, 1); // Set material to white color
        }
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = GetComponent<Renderer>().material;
        
        _material.color = Color.white;
        
    }

    public int getState() {
        return _state;
    }

    public CellType getCellType() {
        return _cellType;
    }

    public Color getCellColor() {
        return _meshRenderer.material.color;
        //return _spriteRenderer.color;
    }

    public bool getColor() { return _isWhite; }

    public void setCube(bool isAlive) {
        _isAlive = isAlive;
        // check if mesh renderer is null
        if (_meshRenderer != null) {
            if (!_isAlive) {
                if (_state <= 0) {
                    // Debug.LogWarning("off");
                    _meshRenderer.enabled = false;
                    return;
                }
                _meshRenderer.material.color = Color.red;
                return;
            }
            _meshRenderer.enabled = true;
            _meshRenderer.material.color = Color.white;
        }
    }

    

    public void setState(int t_state) {
        _state = t_state;
    }

    public void setCellColor(bool isWhite) {
        _isWhite = isWhite;
        if (_meshRenderer != null) {
            if (!_isWhite) {
                _meshRenderer.material.color = Color.red;
                return;
            }
            _meshRenderer.material.color = Color.white;
            return;
        }
        Debug.Log("Null sprite renderer");
    }
}
