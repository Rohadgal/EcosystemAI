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
    bool isHungry = false, isThirsty = false, hasUrge = false, isBusy = false, isSatisfied = true, isInDanger = false;
    GameObject foodTarget, waterTarget, partnerTarget, hunterTarget;
    float closestFood = Mathf.Infinity, closestWater = Mathf.Infinity, closestPartner = Mathf.Infinity, closestThreat = Mathf.Infinity;
    float hungerIncrement = 0.1f, thirstIncrement = 0.12f, urgeIncrement = 0.12f;
    [SerializeField]
    GameObject chickenPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        _animal = GetComponent<Animal>();
        _perceivedFood = new List<GameObject>();
        _perceivedWater = new List<GameObject>();
        _perceivedPartner = new List<GameObject>();
        _perceivedThreats = new List<GameObject>();
        _chickenStates = chickenStates.Wandering;
        //_animal.setThirst(0f);
        //_animal.setHunger(0f);
        
        setGenes();

        survivalSystem();  
    }

    private void FixedUpdate() {
        perceptionManager();
    }

    private void Update() {
       survivalSystem();
    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());

        if(perceivedObjects != null && perceivedObjects.Length > 0) {
            perceivePredator(perceivedObjects);
        }

        if (isHungry) {
            if (perceivedObjects != null && perceivedObjects.Length > 0) {
                seekFood(perceivedObjects);
                //decisionManager();
                //return;
            }
        }
        if (isThirsty) {
            if(perceivedObjects != null && perceivedObjects.Length > 0) {
                seekWater(perceivedObjects);
            }
        }
        if (hasUrge) {
            if (perceivedObjects != null && perceivedObjects.Length > 0) {
                seekPartner(perceivedObjects);
            }
        }

        decisionManager();
    }

    void perceivePredator(Collider[] perceivedObjects) {
        foreach(Collider col in perceivedObjects) {
            if(col.gameObject.CompareTag("cat") || col.gameObject.CompareTag("dog") || col.gameObject.CompareTag("lion")) {
                _perceivedThreats.Add(col.gameObject);
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                if (dist < closestThreat) {
                    closestThreat = dist;
                    hunterTarget = col.gameObject;
                    isInDanger = true;
                }
            }
        }
        bool isSafe = true;
        foreach(GameObject predator in _perceivedThreats) {
            if(Vector3.Distance(transform.position, predator.transform.position) < _animal.getPerceptionRadius()){
                isSafe = false;
            }
        }
        if( isSafe) {
            isInDanger = false;
            closestThreat = Mathf.Infinity;
            hunterTarget = null;
        }
    }

    void seekFood(Collider[] t_perceivedObjects) {
        foreach (Collider col in t_perceivedObjects) {
            if (col.gameObject.CompareTag("grass") && col.gameObject.GetComponent<CapsuleCollider>().enabled && !_perceivedFood.Contains(col.gameObject)) {
                _perceivedFood.Add(col.gameObject);
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                if (dist < closestFood && col.gameObject.GetComponent<CapsuleCollider>().enabled) {
                    closestFood = dist;
                    foodTarget = col.gameObject;
                }
            }
        }
    }

    void seekWater(Collider[] t_perceivedObjects) {
        if(t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
           // closestWater = Mathf.Infinity;
            foreach(Collider col in t_perceivedObjects) {
               
                if(col.gameObject.CompareTag("water") && !_perceivedWater.Contains(col.gameObject)) {
                    _perceivedWater.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if(dist < closestWater) {
                        closestWater = dist;
                        waterTarget = col.gameObject;
                        return; 
                    }
                }
            }
           // decisionManager();
        }
    }

    void seekPartner(Collider[] t_perceivedObjects) {
        if(t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
            foreach(Collider col in t_perceivedObjects) {
                if(col.gameObject.CompareTag("chicken") && col.gameObject.GetComponent<Animal>().getIsFemale() != _animal.getIsFemale() 
                    && !col.gameObject.GetComponent<Chicken>().getIsBusy() && col.gameObject.GetComponent<Chicken>().hasUrge
                    && !_perceivedPartner.Contains(col.gameObject)) {
                    _perceivedPartner.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if(dist < closestPartner) {
                        closestPartner = dist;
                        partnerTarget = col.gameObject;
                        isBusy = true;
                       // return;
                    }
                }
            }
        }
    }

    void decisionManager() {
        //if(t_perceivedObjects != null && _perceivedObjects.Count == 0) { return; }
        //if(_animal.getHunger() > _animal.getThirst()) { 
        //}

        if(isInDanger) {
            if(hunterTarget != null) {
                _animal.setTarget(hunterTarget);
            }
            _chickenStates = chickenStates.Evading;
            movementManager();
            return;
        }

        if (isHungry && foodTarget != null /*&& !hasUrge*/) {
             if (!foodTarget.GetComponent<CapsuleCollider>().enabled) {
                 closestFood = Mathf.Infinity;
                 return;
             }
             _animal.setTarget(foodTarget);
             _chickenStates = chickenStates.Seeking;
             if (Vector3.Distance(transform.position, foodTarget.transform.position) <= 2f) {
                 _chickenStates = chickenStates.Eating;
                 
                 actionManager();
                 return;
             }
        }

        if (isThirsty && waterTarget != null /*&& !hasUrge*/) {
            
            _animal.setTarget(waterTarget);
            _chickenStates = chickenStates.Seeking;

            if(Vector3.Distance(transform.position, waterTarget.transform.position) <= 2f) {
                _chickenStates = chickenStates.Drinking;
                  
                actionManager();
                return;
            }
        }

        if(hasUrge && partnerTarget != null) {
            _animal.setTarget(partnerTarget);
            _chickenStates = chickenStates.Seeking;

            if(Vector3.Distance(transform.position, partnerTarget.transform.position) <= 2f) {
             
                _chickenStates = chickenStates.Reproducing;
                actionManager();
                return;
            }
        }

        if(isSatisfied) {
            _chickenStates = chickenStates.Wandering;
        }
        movementManager();
        actionManager();
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
               // _perceivedWater.Clear();
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
        }
    }

    void setGenes() {
        _animal.setGender(Random.Range(0, 150) < 75f);
        //Debug.Log(_animal.getIsFemale());
        _animal._gene.feelHungry = Random.Range(20f, 40f);
        _animal._gene.feelThirst = Random.Range(30f, 50f);
        _animal._gene.feelUrge = Random.Range(20,60f);
    }

    void eat(GameObject _food) {
        isHungry = false;
        _food.GetComponent<CapsuleCollider>().enabled = false;
        _food.GetComponent<MeshRenderer>().enabled = false;
        _perceivedFood.Clear();
        closestFood = Mathf.Infinity;
       // foodTarget = null;
        //if (!isThirsty) {
        //    _animal.setTarget(null);
        //}
        _animal.setHunger(0f);
        isSatisfied = true;
    }

    void drink(GameObject _water) {
        isThirsty = false;
        _perceivedWater.Clear();
        closestWater = Mathf.Infinity;
        //waterTarget = null;
        if (!isHungry) {
            _animal.setTarget(null);
        }
        _animal.setThirst(0f);
        isSatisfied=true;
    }

    void reproduce(GameObject t_partner) {
        if(_animal.getIsFemale()) {
            Debug.Log("Reproduce");
            _animal.procreate(t_partner.GetComponent<Animal>(), chickenPrefab);
            //hasUrge = false;
            //t_partner.GetComponent<Chicken>().setUrge(false);
        }
        isBusy = false;
        _perceivedPartner.Clear();
        partnerTarget = null;
        _animal.setUrge(0f);
        hasUrge = false;

    }

    public bool getIsBusy() { return isBusy; }
    
    public void setUrge(bool t_hasUrge) {
        hasUrge =t_hasUrge;
    }
}
