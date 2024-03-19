using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum chickenStates { None, Wandering, Seeking, Drinking, Eating, Reproducing, Evading, Dead}


[RequireComponent(typeof(Animal))]
public class Chicken : MonoBehaviour
{
    Animal _animal;
    chickenStates _chickenStates;

    public List<GameObject> _perceivedFood, _perceivedWater, _perceivedPartner, _perceivedThreats;

    bool isHungry = false, isThirsty = false, hasUrge = false,/*isBusy = false,*/ isSatisfied = true, isInDanger = false;

    GameObject foodTarget, waterTarget, partnerTarget, hunterTarget;

    float closestPartner = Mathf.Infinity, closestThreat = Mathf.Infinity;

    float hungerIncrement = 0.25f, thirstIncrement = 0.3f, urgeIncrement = 0.6f;

    [SerializeField]
    GameObject chickenPrefab;
   
    bool doCoroutine = true;

    public float rotationSpeed = 120f;

    public float raycastDistance = 3f;
    public LayerMask obstacleLayer;

    // Start is called before the first frame update
    void Start()
    {
        _animal = GetComponent<Animal>();
        _perceivedFood = new List<GameObject>();
        _perceivedWater = new List<GameObject>();
        _perceivedPartner = new List<GameObject>();
        _perceivedThreats = new List<GameObject>();
        _chickenStates = chickenStates.Wandering;

        if (!_animal.isChild) {

          setGenes();
        }

        survivalSystem();  
    }
    IEnumerator perceive() {
        doCoroutine = false;
        yield return new WaitForSeconds(.5f);
        survivalSystem();
        perceptionManager();
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, obstacleLayer)) {
            if(hit.collider.gameObject != waterTarget || isHungry || hasUrge) {
                avoidObstacle(hit);
            }
        }
        doCoroutine = true;
    }

    void avoidObstacle(RaycastHit hit) {
        // Calculate torque based on the rotation operation
        Vector3 torque = Vector3.up * rotationSpeed;
        Debug.Log("this");
        // Apply torque to the Rigidbody
        _animal.rb.AddTorque(torque);
    }

    // Update is called once per frame
    void Update() {
        // survivalSystem();
        if (doCoroutine) {

            StartCoroutine(perceive());
        }

    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());

        if(perceivedObjects != null && perceivedObjects.Length > 0) {
            perceivePredator(perceivedObjects);

            if (!isInDanger) {

                if (isHungry && !foodTarget) { 
                    seekFood(perceivedObjects);
                } else if (isThirsty && !waterTarget) {
                    seekWater(perceivedObjects);
                } else if (hasUrge && !partnerTarget) {
                    seekPartner(perceivedObjects);
                }
            }
        }
        decisionManager();
    }

    void perceivePredator(Collider[] perceivedObjects) {
        //closestThreat = Mathf.Infinity;
        foreach(Collider col in perceivedObjects) {
            if(col.gameObject.CompareTag("cat")) {
                _perceivedThreats.Add(col.gameObject);
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                if (dist < closestThreat) {
                    closestThreat = dist;
                    hunterTarget = col.gameObject;
                    isInDanger = true;
                    return;
                }
            }
        }
        bool isSafe = true;
        foreach(GameObject predator in _perceivedThreats) {
            if(Vector3.Distance(transform.position, predator.transform.position) < _animal.getPerceptionRadius()){
                if (predator.activeSelf) {
                    isSafe = false;
                }
            }
        }
        if( isSafe) {
            isInDanger = false;
            _perceivedThreats.Clear();
            closestThreat = Mathf.Infinity;
            hunterTarget = null;
            _animal.setTarget(null);
        }
    }

    void seekFood(Collider[] t_perceivedObjects) {
        foreach (Collider col in t_perceivedObjects) {
            if (col.gameObject.CompareTag("grass") && col.gameObject.GetComponent<CapsuleCollider>().enabled && !_perceivedFood.Contains(col.gameObject)) {
                _perceivedFood.Add(col.gameObject);
                    foodTarget = col.gameObject;
                    return;
            }
        }
    }

    void seekWater(Collider[] t_perceivedObjects) {
        foreach(Collider col in t_perceivedObjects) {
            if(col.gameObject.CompareTag("water") && !_perceivedWater.Contains(col.gameObject)) {
                _perceivedWater.Add(col.gameObject);
                    waterTarget = col.gameObject;
                    return; 
            }
        }
    }

    void seekPartner(Collider[] t_perceivedObjects) {
        if(t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
            foreach(Collider col in t_perceivedObjects) {
                if(col.gameObject.CompareTag("chicken") && col.gameObject.GetComponent<Animal>().getIsFemale() != _animal.getIsFemale() 
                    && col.gameObject.GetComponent<Chicken>().hasUrge
                    && !_perceivedPartner.Contains(col.gameObject)) {
                    _perceivedPartner.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if(dist < closestPartner) {
                        closestPartner = dist;
                        partnerTarget = col.gameObject;
                        return;
                    }
                }
            }
        }
    }

    void decisionManager() {
        if(isInDanger && hunterTarget != null) {
            
            _animal.setTarget(hunterTarget);
            
            _chickenStates = chickenStates.Evading;
            movementManager();
            return;
        }

        else if (isHungry && foodTarget != null) {
             _animal.setTarget(foodTarget);
             _chickenStates = chickenStates.Seeking;
           
             if (Vector3.Distance(transform.position, foodTarget.transform.position) <= 2f) {
                 _chickenStates = chickenStates.Eating;
                 
                 actionManager();
                 return;
             }
            movementManager();
            return;
        }

        else if (isThirsty && waterTarget != null) {
            
            _animal.setTarget(waterTarget);
            _chickenStates = chickenStates.Seeking;

            if(Vector3.Distance(transform.position, waterTarget.transform.position) <= 2f) {
                _chickenStates = chickenStates.Drinking;
                  
                actionManager();
                return;
            }
            movementManager();
            return;
        }

       else if(hasUrge && partnerTarget != null) {
            _animal.setTarget(partnerTarget);
            _chickenStates = chickenStates.Seeking;

            if(Vector3.Distance(transform.position, partnerTarget.transform.position) <= 2f) {
             
                _chickenStates = chickenStates.Reproducing;
                actionManager();
                return;
            }
            movementManager();
            return;
        }

        if(isSatisfied) {
            _chickenStates = chickenStates.Wandering;
        }
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
            case chickenStates.Evading:
                _animal.ChangeAnimalState(AnimalState.Evade);
                if (_animal.getTarget() == null) {
                    _animal.ChangeAnimalState(AnimalState.Wander);
                }
                break;
            default: break;
        }
    }

    void actionManager() {
        switch(_chickenStates) {
            case chickenStates.Eating:
                _animal.ChangeAnimalState(AnimalState.Eat);
                eat(foodTarget);
                break;
            case chickenStates.Drinking:
                _animal.ChangeAnimalState(AnimalState.Eat);
                drink(waterTarget);
                break;
            case chickenStates.Reproducing:
                _animal.ChangeAnimalState(AnimalState.Reproduce);
                reproduce(partnerTarget); 
                break;
             default : break;
        }
    }

    void survivalSystem() {
        _animal._gene.hungerSystem(_animal, hungerIncrement);
        _animal._gene.thirstSystem(_animal, thirstIncrement);
        _animal._gene.urgeSystem(_animal, urgeIncrement);

        if (_animal.getHunger() > _animal._gene.feelHungry) {
            //Debug.LogWarning("hungry: " + isHungry);
            isHungry = true;
            isSatisfied = false;
        }
        if (_animal.getThirst() > _animal._gene.feelThirst) {
            isThirsty = true;
            isSatisfied = false;
        }
        if (_animal.getUrge() > _animal._gene.feelUrge) {
            hasUrge = true;
           //isSatisfied = false;
        }
    }

    void setGenes() {
        _animal.setGender(Random.Range(0, 150) < 75f);
        //Debug.Log(_animal.getIsFemale());
        _animal._gene.feelHungry = Random.Range(20f, 40f);
        _animal._gene.feelThirst = Random.Range(30f, 50f);
        _animal._gene.feelUrge = Random.Range(20,60f);

        Debug.Log("chicken hunger: " + _animal._gene.feelHungry);
    }

    void eat(GameObject _food) {
        isHungry = false;
        _food.GetComponent<CapsuleCollider>().enabled = false;
        _food.GetComponent<MeshRenderer>().enabled = false;
        _perceivedFood.Clear();
        foodTarget = null;
        //if (!isThirsty) {
        //}
         _animal.setTarget(null);
        _animal.setHunger(0f);
        isSatisfied = true;
    }

    void drink(GameObject _water) {
        isThirsty = false;
        _perceivedWater.Clear();
        waterTarget = null;
         _animal.setTarget(null);
        _animal.setThirst(0f);
        isSatisfied=true;
    }

    void reproduce(GameObject t_partner) {
        if(_animal.getIsFemale()) {
            _animal.procreate(t_partner.GetComponent<Animal>(), chickenPrefab);
        }
        _perceivedPartner.Clear();
        partnerTarget = null;
        _animal.setTarget(null);
        _animal.setUrge(0f);
        hasUrge = false;

    }
    
    public void setUrge(bool t_hasUrge) {
        hasUrge =t_hasUrge;
    }
}
