using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum chickenStates { None, Wandering, Seeking, Eating, Evading, Dead}


[RequireComponent(typeof(Animal))]
public class Chicken : MonoBehaviour
{
    Animal _animal;
    chickenStates _chickenStates;
    public List<GameObject> _perceivedObjects;

    // Start is called before the first frame update
    void Start()
    {
        _animal = GetComponent<Animal>();
        _perceivedObjects = new List<GameObject>();
        if(_animal.rb == null) {
            _animal.rb = GetComponent<Rigidbody>();
        }
        _chickenStates = chickenStates.Wandering;
        
    }

    private void FixedUpdate() {
        perceptionManager();
    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());
        foreach(Collider col in perceivedObjects) {
            if(col.gameObject.CompareTag("grass") && !_perceivedObjects.Contains(col.gameObject)) {
                _perceivedObjects.Add(col.gameObject);
            }
        }
        decisionManager(_perceivedObjects);
    }

    void decisionManager(List<GameObject> t_perceivedObjects) {
        if(t_perceivedObjects != null && _perceivedObjects.Count == 0) { return; }

        //foreach(GameObject grass in t_perceivedObjects) {
        //    this.transform.LookAt(grass.transform.position);
        //    _animal.setTarget(grass.gameObject);
        //    _chickenStates = chickenStates.Seeking;

        //}

        //switch( _chickenStates ) {
        //    case chickenStates.Wandering: movementManager(); break;
        //    case chickenStates.Seeking: movementManager(); break;
        //}
        movementManager();
    }

    void movementManager() {
       
        switch( _chickenStates ) {
            case chickenStates.None: 
                _animal.ChangeAnimalState(AnimalState.Idle); 
                break;
            case chickenStates.Wandering:
                //Debug.Log("this");
                _animal.ChangeAnimalState(AnimalState.Wander);
                break;
            case chickenStates.Seeking:
                _animal.ChangeAnimalState(AnimalState.Seek);
                break;
        }
    }
}
