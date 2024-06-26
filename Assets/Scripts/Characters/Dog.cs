using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum dogStates { None, Wandering, Seeking, Drinking, Eating, Reproducing, Evading, Dead }


public class Dog : MonoBehaviour {
    Animal _animal;
    dogStates _dogStates;
    public List<GameObject> _perceivedFood, _perceivedWater, _perceivedPartner, _perceivedThreats;
    bool isHungry = false, isThirsty = false, hasUrge = false,/* isBusy = false, */isSatisfied = true, isInDanger;
    GameObject foodTarget, waterTarget, partnerTarget, hunterTarget;
    float closestPartner = Mathf.Infinity, closestThreat = Mathf.Infinity;
    float hungerIncrement = 0.25f, thirstIncrement = 0.3f, urgeIncrement = 0.7f;
    [SerializeField]
    GameObject dogPrefab;
    [SerializeField]
    AudioSource audioSource;
    AudioClip clip;
    bool doCoroutine = true;

    public float raycastDistance = 3f;
    public LayerMask obstacleLayer;

    public float rotationSpeed = 120f;

    // Start is called before the first frame update
    void Start() {
        if(audioSource != null) {
            clip = audioSource.clip;
        }
        _animal = GetComponent<Animal>();
        _perceivedFood = new List<GameObject>();
        _perceivedWater = new List<GameObject>();
        _perceivedPartner = new List<GameObject>();
        _perceivedThreats = new List<GameObject>();
        _dogStates = dogStates.Wandering;

        setGenes();

        survivalSystem();
    }
    IEnumerator perceive() {
        doCoroutine = false;
        yield return new WaitForSeconds(.5f);
        survivalSystem();
        perceptionManager();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, obstacleLayer)) {
            if (hit.collider.gameObject == waterTarget) {

            } else {
                avoidObstacle(hit);
            }
        }
        doCoroutine = true;
    }

    void avoidObstacle(RaycastHit hit) {
        // Calculate torque based on the rotation operation
        Vector3 torque = Vector3.up * rotationSpeed;

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

        if(perceivedObjects != null &&  perceivedObjects.Length > 0 ) {
            perceivedPredator(perceivedObjects);
            if (!isInDanger) {
                if (isHungry) {
                    seekFood(perceivedObjects);
                }
                else if (isThirsty) {
                    seekWater(perceivedObjects);
                }
                else if (hasUrge) {
                    seekPartner(perceivedObjects);
                }
            }
        }
        decisionManager();
    }

    void perceivedPredator(Collider[] perceivedObjects) {
        foreach(Collider col in perceivedObjects) {
            if (col.gameObject.CompareTag("lion")) {
                _perceivedThreats.Add(col.gameObject);
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                if(dist < closestThreat) {
                    closestThreat = dist;
                    hunterTarget = col.gameObject;
                    isInDanger = true;
                }
            }
        }
        bool isSafe = true;
        foreach(GameObject predator in _perceivedThreats) {
            if(Vector3.Distance(transform.position, predator.transform.position) < _animal.getPerceptionRadius()) {
                if (predator.activeSelf) {
                    isSafe = false;
                }
            }
        }
        if(isSafe) {
            isInDanger = false;
            _perceivedThreats.Clear();
            closestThreat = Mathf.Infinity;
            hunterTarget = null;
            _animal.setTarget(null);
        }
    }

    void seekFood(Collider[] t_perceivedObjects) {
        foreach (Collider col in t_perceivedObjects) {
            if (col.gameObject.CompareTag("cat") && !_perceivedFood.Contains(col.gameObject)) {
                _perceivedFood.Add(col.gameObject);
                    foodTarget = col.gameObject;
                    return;
            }
        }
    }

    void seekWater(Collider[] t_perceivedObjects) {
        foreach (Collider col in t_perceivedObjects) {
            if (col.gameObject.CompareTag("water") && !_perceivedWater.Contains(col.gameObject)) {
                _perceivedWater.Add(col.gameObject);
                    waterTarget = col.gameObject;
                    return;
                
            }
        }
    }

    void seekPartner(Collider[] t_perceivedObjects) {
        if (t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
            foreach (Collider col in t_perceivedObjects) {
                if (col.gameObject.CompareTag("dog") && col.gameObject.GetComponent<Animal>().getIsFemale() != _animal.getIsFemale()
                   /* && !col.gameObject.GetComponent<Dog>().getIsBusy()*/ && col.gameObject.GetComponent<Dog>().hasUrge
                    && !_perceivedPartner.Contains(col.gameObject)) {
                    _perceivedPartner.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if (dist < closestPartner) {
                        closestPartner = dist;
                        partnerTarget = col.gameObject;
                        //isBusy = true;
                        return;
                        // return;
                    }
                }
            }
        }
    }

    void decisionManager() {

        if(isInDanger && hunterTarget != null) {
            
            _animal.setTarget(hunterTarget);
            
            _dogStates = dogStates.Evading;
            movementManager();
            return;
        }

        else if (isHungry && foodTarget != null) {

            if (!foodTarget.activeSelf) {
                foodTarget = null;
                _perceivedFood.Clear();
                perceptionManager();
                return;
            }

            _animal.setTarget(foodTarget);
            _dogStates = dogStates.Seeking;

            if (Vector3.Distance(transform.position, foodTarget.transform.position) <= 2f) {
                _dogStates = dogStates.Eating;

                actionManager();
                return;
            }
            movementManager();
            return;
        }

        else if (isThirsty && waterTarget != null) {

            _animal.setTarget(waterTarget);
            _dogStates = dogStates.Seeking;

            if (Vector3.Distance(transform.position, waterTarget.transform.position) <= 2f) {
                _dogStates = dogStates.Drinking;

                actionManager();
                return;
            }
            movementManager();
            return;
        }

        else if (hasUrge && partnerTarget != null) {
            _animal.setTarget(partnerTarget);
            _dogStates = dogStates.Seeking;
            if (audioSource != null && Random.Range(0, 100) < 15) {
                audioSource.PlayOneShot(clip);
            }

            if (Vector3.Distance(transform.position, partnerTarget.transform.position) <= 2f) {

                _dogStates = dogStates.Reproducing;
                actionManager();
                return;
            }
            movementManager();
            return;
        }

        if (isSatisfied) {
            _dogStates = dogStates.Wandering;
        }

        movementManager();
    }

    void movementManager() {

        switch (_dogStates) {
            case dogStates.None:
                _animal.ChangeAnimalState(AnimalState.Idle);
                break;
            case dogStates.Wandering:
                _animal.ChangeAnimalState(AnimalState.Wander);
                break;
            case dogStates.Seeking:
                _animal.ChangeAnimalState(AnimalState.Seek);
                break;
            case dogStates.Evading:
                _animal.ChangeAnimalState(AnimalState.Evade);
                if (_animal.getTarget() == null) {
                    _animal.ChangeAnimalState(AnimalState.Wander);
                }
                break;
            default: break;
        }
    }

    void actionManager() {
        switch (_dogStates) {
            case dogStates.Eating:
                _animal.ChangeAnimalState(AnimalState.Eat);
                eat(foodTarget);
                break;
            case dogStates.Drinking:
                _animal.ChangeAnimalState(AnimalState.Eat);
                drink(waterTarget);
                break;
            case dogStates.Reproducing:
                _animal.ChangeAnimalState(AnimalState.Reproduce);
                reproduce(partnerTarget);
                break;
            default: break;
        }
    }

    void survivalSystem() {
        _animal._gene.hungerSystem(_animal, hungerIncrement);
        _animal._gene.thirstSystem(_animal, thirstIncrement);
        _animal._gene.urgeSystem(_animal, urgeIncrement);

        if (_animal.getHunger() > _animal._gene.feelHungry) {
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
      //  Debug.Log(_animal.getIsFemale());
        _animal._gene.feelHungry = Random.Range(10, 50);
        _animal._gene.feelThirst = Random.Range(10, 50);
        _animal._gene.feelUrge = Random.Range(10, 50);
    }

    void eat(GameObject _food) {
        isHungry = false;
        _food.SetActive(false);
        //_food.GetComponent<MeshRenderer>().enabled = false;
        _perceivedFood.Clear();
        foodTarget = null;
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
        isSatisfied = true;
    }

    void reproduce(GameObject t_partner) {
        if (_animal.getIsFemale()) {
            _animal.procreate(t_partner.GetComponent<Animal>(), dogPrefab);
        }
        //isBusy = false;
        _perceivedPartner.Clear();
        partnerTarget = null;
        _animal.setTarget(null);
        _animal.setUrge(0f);
        hasUrge = false;
    }

    //public bool getIsBusy() { return isBusy; }

    public void setUrge(bool t_hasUrge) {
        hasUrge = t_hasUrge;
    }
}
