using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum dogStates { None, Wandering, Seeking, Drinking, Eating, Reproducing, Evading, Dead }


public class Dog : MonoBehaviour {
    Animal _animal;
    dogStates _dogStates;
    public List<GameObject> _perceivedFood, _perceivedWater, _perceivedPartner;
    bool isHungry = false, isThirsty = false, hasUrge = false, isBusy = false, isSatisfied = true;
    GameObject foodTarget, waterTarget, partnerTarget;
    float closestFood = Mathf.Infinity, closestWater = Mathf.Infinity, closestPartner = Mathf.Infinity;
    float hungerIncrement = 0.1f, thirstIncrement = 0.12f, urgeIncrement = 0.12f;
    [SerializeField]
    GameObject dogPrefab;

    // Start is called before the first frame update
    void Start() {
        _animal = GetComponent<Animal>();
        _perceivedFood = new List<GameObject>();
        _perceivedWater = new List<GameObject>();
        _dogStates = dogStates.Wandering;

        setGenes();

        survivalSystem();
    }

    private void FixedUpdate() {
        perceptionManager();
    }


    // Update is called once per frame
    void Update() {
        survivalSystem();
    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());
        if (isHungry) {
            if (perceivedObjects != null && perceivedObjects.Length != 0) {
                seekFood(perceivedObjects);
                //decisionManager();
                //return;
            }
        }
        if (isThirsty) {
            if (perceivedObjects != null && perceivedObjects.Length != 0) {
                seekWater(perceivedObjects);
                // decisionManager();
                // return;
            }
        }
        if (hasUrge) {
            if (perceivedObjects != null && perceivedObjects.Length != 0) {
                seekPartner(perceivedObjects);
                // decisionManager();
                // return;
            }
        }

        decisionManager();
    }

    void seekFood(Collider[] t_perceivedObjects) {

        if (t_perceivedObjects != null && t_perceivedObjects.Length != 0) {

            foreach (Collider col in t_perceivedObjects) {
                if (col.gameObject.CompareTag("chicken") && !_perceivedFood.Contains(col.gameObject)
                    || col.gameObject.CompareTag("cat") && !_perceivedFood.Contains(col.gameObject)) {
                    _perceivedFood.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if (dist < closestFood) {
                        closestFood = dist;
                        foodTarget = col.gameObject;
                    }
                }
            }
        }

    }

    void seekWater(Collider[] t_perceivedObjects) {
        if (t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
            // closestWater = Mathf.Infinity;
            foreach (Collider col in t_perceivedObjects) {

                if (col.gameObject.CompareTag("water") && !_perceivedWater.Contains(col.gameObject)) {
                    _perceivedWater.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if (dist < closestWater) {
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
        if (t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
            foreach (Collider col in t_perceivedObjects) {
                if (col.gameObject.CompareTag("dog") && col.gameObject.GetComponent<Animal>().getIsFemale() != _animal.getIsFemale()
                    && !col.gameObject.GetComponent<Dog>().getIsBusy() && col.gameObject.GetComponent<Dog>().hasUrge
                    && !_perceivedPartner.Contains(col.gameObject)) {
                    _perceivedPartner.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if (dist < closestPartner) {
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

        if (isHungry && foodTarget != null /*&& !hasUrge*/) {

            _animal.setTarget(foodTarget);
            _dogStates = dogStates.Seeking;

            if (Vector3.Distance(transform.position, foodTarget.transform.position) <= 2f) {
                _dogStates = dogStates.Eating;

                actionManager();
                return;
            }
        }

        if (isThirsty && waterTarget != null /*&& !hasUrge*/) {

            _animal.setTarget(waterTarget);
            _dogStates = dogStates.Seeking;

            if (Vector3.Distance(transform.position, waterTarget.transform.position) <= 2f) {
                _dogStates = dogStates.Drinking;

                actionManager();
                return;
            }
        }

        if (hasUrge && partnerTarget != null) {
            _animal.setTarget(partnerTarget);
            _dogStates = dogStates.Seeking;

            if (Vector3.Distance(transform.position, partnerTarget.transform.position) <= 2f) {

                _dogStates = dogStates.Reproducing;
                actionManager();
                return;
            }
        }

        if (isSatisfied) {
            _dogStates = dogStates.Wandering;
        }

        movementManager();
        actionManager();
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
                // _perceivedWater.Clear();
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
        Debug.Log(_animal.getIsFemale());
        _animal._gene.feelHungry = 20f;
        _animal._gene.feelThirst = 40f;
        _animal._gene.feelUrge = 60f;
    }

    void eat(GameObject _food) {
        isHungry = false;
        _food.SetActive(false);
        //_food.GetComponent<MeshRenderer>().enabled = false;
        _perceivedFood.Clear();
        closestFood = Mathf.Infinity;
        _animal.setHunger(0f);
        isSatisfied = true;
    }

    void drink(GameObject _water) {
        isThirsty = false;
        _perceivedWater.Clear();
        closestWater = Mathf.Infinity;
        if (!isHungry) {
            _animal.setTarget(null);
        }
        _animal.setThirst(0f);
        isSatisfied = true;
    }

    void reproduce(GameObject t_partner) {
        if (_animal.getIsFemale()) {
            Debug.Log("Reproduce");
            _animal.procreate(t_partner.GetComponent<Animal>(), dogPrefab);
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
        hasUrge = t_hasUrge;
    }
}
