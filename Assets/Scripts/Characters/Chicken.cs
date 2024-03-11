using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum chickenStates { None, Wandering, Seeking, Drinking, Eating, Evading, Dead}


[RequireComponent(typeof(Animal))]
public class Chicken : MonoBehaviour
{
    Animal _animal;
    chickenStates _chickenStates;
    public List<GameObject> _perceivedObjects;
    bool isHungry = false, isThirsty = false, isSatisfied = true;
    GameObject target;
    float closestFood = Mathf.Infinity, closestWater = Mathf.Infinity;

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
        _animal.setThirst(0f);
        _animal.setHunger(0f);
        hungerSystem();
        thirstSystem();
        
    }

    private void FixedUpdate() {
        perceptionManager();
    }

    private void Update() {
       hungerSystem();
       thirstSystem();
       survivalSystem();
    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());
        //if(isHungry) {
        //    seekFood(perceivedObjects);
            
        //}
        if (isThirsty) {
            if(perceivedObjects != null && perceivedObjects.Length != 0) {
                seekWater(perceivedObjects);
            }
        }

        decisionManager();
    }

    void seekFood(Collider[] t_perceivedObjects) {
        foreach (Collider col in t_perceivedObjects) {
            if (col.gameObject.CompareTag("grass") && col.gameObject.GetComponent<CapsuleCollider>().enabled && !_perceivedObjects.Contains(col.gameObject)) {
                _perceivedObjects.Add(col.gameObject);
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                if (dist < closestFood && col.gameObject.GetComponent<CapsuleCollider>().enabled) {
                    closestFood = dist;
                    target = col.gameObject;
                    //Debug.Log("Chicken: " + gameObject.name + " Distance: " + Vector3.Distance(transform.position, target.gameObject.transform.position));
                }
            }
        }
    }

    void seekWater(Collider[] t_perceivedObjects) {
        if(t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
            foreach(Collider col in t_perceivedObjects) {
               
                if(col.gameObject.CompareTag("water") && !_perceivedObjects.Contains(col.gameObject)) {
                    Debug.Log("thi");
                    _perceivedObjects.Add(col.gameObject) ;
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if(dist < closestWater) {
                        closestWater = dist;
                        target = col.gameObject;
                    }
                }
            }
            decisionManager();
        }
    }

    void decisionManager() {
        //if(t_perceivedObjects != null && _perceivedObjects.Count == 0) { return; }

        //if (isHungry) {
        //    if (!target.GetComponent<CapsuleCollider>().enabled) {
        //        closestFood = Mathf.Infinity;
        //        return;
        //    }

        //    _animal.setTarget(target);
        //    _chickenStates = chickenStates.Seeking;

            

        //    if(Vector3.Distance(transform.position, target.transform.position) <= 2f) {
        //        _chickenStates = chickenStates.Eating;
        //        isHungry = false;
        //        actionManager();
        //        return;
        //    }
        //}

        if(isThirsty) {
            _animal.setTarget(target);
            _chickenStates = chickenStates.Seeking;

            if(Vector3.Distance(transform.position, target.transform.position) <= 2f) {
                _chickenStates = chickenStates.Drinking;
                isThirsty = false;  
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
                Debug.Log("seek");
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
            case chickenStates.Drinking:
                _animal.ChangeAnimalState(AnimalState.Eat);
                _perceivedObjects.Clear();
                drink(target);
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

    void thirstSystem() {
        float thirstIncrement = 0.12f;
        float currentThirstLevel = _animal.getThirst() + thirstIncrement;
        _animal.setThirst(currentThirstLevel);
        _animal.updateThirstBar(_animal.getThirst());
    }

    void survivalSystem() {
        float feelHungry = 20f;
        float feelThirst = 40f;
        //if (_animal.getHunger() > feelHungry) {
        //    //Debug.LogWarning("hungry: " + isHungry);
        //    isHungry = true;
        //    isSatisfied = false;
        //}
        if(_animal.getThirst() > feelThirst) {
            isThirsty = true;
            isSatisfied = false;
        }
    }

    void eat(GameObject _food) {
        _food.GetComponent<CapsuleCollider>().enabled = false;
        _food.GetComponent<MeshRenderer>().enabled = false;
        closestFood = Mathf.Infinity;
        _animal.setTarget(null);
        _animal.setHunger(0f);
        isSatisfied = true;
    }

    void drink(GameObject _water) {
        closestWater = Mathf.Infinity;
        _animal.setTarget(null);
        _animal.setThirst(0f);
        isSatisfied=true;
    }
}
