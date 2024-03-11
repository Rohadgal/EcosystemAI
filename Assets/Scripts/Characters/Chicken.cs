using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum chickenStates { None, Wandering, Seeking, Eating, Evading, Dead}


[RequireComponent(typeof(Animal))]
public class Chicken : MonoBehaviour
{
    Animal _animal;
    chickenStates _chickenStates;
    public List<GameObject> _perceivedObjects;
    bool isHungry = false, isSatisfied = true;
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
       // _animal.updateHungerBar(_animal.getHunger());

        _animal = GetComponent<Animal>();
        _perceivedObjects = new List<GameObject>();
        if(_animal.rb == null) {
            _animal.rb = GetComponent<Rigidbody>();
        }
        _chickenStates = chickenStates.Wandering;
        _animal.setHunger(0f);
        hungerSystem();
        
    }

    private void FixedUpdate() {
        perceptionManager();
    }

    private void Update() {
       hungerSystem();
       survivalSystem();
    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());
        if(isHungry) {
            float closestFood = Mathf.Infinity;
            foreach(Collider col in perceivedObjects) {
                if(col.gameObject.CompareTag("grass") && !_perceivedObjects.Contains(col.gameObject)) {
                    _perceivedObjects.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if(dist < closestFood) {
                        closestFood = dist;
                        target = col.gameObject;
                        Debug.Log("Chicken: " + gameObject.name + " Distance: " + Vector3.Distance(transform.position, target.gameObject.transform.position));
                    }
                }
            }
        }
        decisionManager(_perceivedObjects);
    }

    void decisionManager(List<GameObject> t_perceivedObjects) {
        //if(t_perceivedObjects != null && _perceivedObjects.Count == 0) { return; }

        if (isHungry) {
            _animal.setTarget(target);
            _chickenStates = chickenStates.Seeking;

            if(Vector3.Distance(transform.position, target.transform.position) <= 2f) {
                _chickenStates = chickenStates.Eating;
                isHungry = false;
                actionManager();
                return;
            }
        }

        if(isSatisfied) {
            _chickenStates = chickenStates.Wandering;
        }

        //foreach(GameObject grass in t_perceivedObjects) {
        //    this.transform.LookAt(grass.transform.position);
        //    _animal.setTarget(grass.gameObject);
        //    _chickenStates = chickenStates.Seeking;

        //}

        //switch (_chickenStates) {
        //    case chickenStates.Wandering: movementManager(); break;
        //    case chickenStates.Seeking: movementManager(); break;
        //    default: break;
        //}
        movementManager();
    }

    void movementManager() {
       
        switch( _chickenStates ) {
            case chickenStates.None: 
                _animal.ChangeAnimalState(AnimalState.Idle); 
                break;
            case chickenStates.Wandering:
                _animal.ChangeAnimalState(AnimalState.Wander);
                break;
            case chickenStates.Seeking:
                _animal.ChangeAnimalState(AnimalState.Seek);
                break;
            default: break;
        }
    }

    void actionManager() {
        switch(_chickenStates) {
            case chickenStates.Eating:
                _animal.ChangeAnimalState(AnimalState.Eat);
                _perceivedObjects.Clear();
                eat(target);
                break;
             default : break;
        }
    }

    void hungerSystem() {
        float hungerIncrement = 0.1f;
        float currentHungerLevel = _animal.getHunger() + hungerIncrement;
        _animal.setHunger(currentHungerLevel);
        _animal.updateHungerBar(_animal.getHunger());
    }

    void survivalSystem() {
        float feelHungry = 20f;
        if(_animal.getHunger() > feelHungry) {
            Debug.LogWarning("hungry: " + isHungry);
            isHungry = true;
            isSatisfied = false;
        }
    }

    void eat(GameObject _food) {
        _food.GetComponent<CapsuleCollider>().enabled = false;
        _food.GetComponent<MeshRenderer>().enabled = false;
        _animal.setHunger(0f);
        isSatisfied = true;
    }
}
