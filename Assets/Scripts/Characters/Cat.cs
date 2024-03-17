using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum catStates { None, Wandering, Seeking, Drinking, Eating, Reproducing, Evading, Dead}

[RequireComponent(typeof(Animal))]
public class Cat : MonoBehaviour
{
    Animal _animal;
    catStates _catStates;
    public List<GameObject> _perceivedFood, _perceivedWater, _perceivedPartner, _perceivedThreats;
    bool isHungry = false, isThirsty = false, hasUrge = false, isBusy = false, isSatisfied = true, isInDanger;
    GameObject foodTarget, waterTarget, partnerTarget, hunterTarget;
    float closestFood = Mathf.Infinity, closestWater = Mathf.Infinity, closestPartner = Mathf.Infinity, closestThreat = Mathf.Infinity;
    float hungerIncrement = 0.7f, thirstIncrement = 0.7f, urgeIncrement = 0.24f;
    [SerializeField]
    GameObject catPrefab;
    bool doCoroutine = true;

    // Start is called before the first frame update
    void Start()
    {
        _animal = GetComponent<Animal>();
        _perceivedFood = new List<GameObject>();
        _perceivedWater = new List<GameObject>();
        _perceivedPartner = new List<GameObject>();
        _perceivedThreats = new List<GameObject>();
        _catStates = catStates.Wandering;

        setGenes();

        survivalSystem();
    }

    //private void FixedUpdate() {
    //    perceptionManager();
    //}

    IEnumerator perceive() {
        doCoroutine = false;
        yield return new WaitForSeconds(.5f);
        perceptionManager();
        survivalSystem();
        doCoroutine = true;
    }

    // Update is called once per frame
    void Update()
    {
        // survivalSystem();
        if (doCoroutine) {

            StartCoroutine(perceive());
        }
    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());

        if(perceivedObjects != null && perceivedObjects.Length > 0) {

            perceivedPredator(perceivedObjects);

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
        decisionManager();
    }

    void perceivedPredator(Collider[] perceivedObjects) {
        foreach(Collider col in perceivedObjects) {
            if(col.gameObject.CompareTag("dog") || col.gameObject.CompareTag("lion")) {
                _perceivedThreats.Add(col.gameObject); 
                float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                if(dist < closestThreat) {
                    closestThreat = dist;
                    hunterTarget = col.gameObject;
                    isInDanger = true;
                    return;
                }
            }
        }
        bool isSafe = true;
        foreach(GameObject predator in _perceivedThreats) {
            if(Vector3.Distance(transform.position, predator.transform.position) < _animal.getPerceptionRadius()) {
                isSafe = false;
            }
        }
        if(isSafe) {
            isInDanger = false;
            _perceivedThreats.Clear();
            closestThreat = Mathf.Infinity;
            hunterTarget = null;
        }
    }

    void seekFood(Collider[] t_perceivedObjects) {
        if(t_perceivedObjects != null && t_perceivedObjects.Length != 0) {
            foreach (Collider col in t_perceivedObjects) {
                if (col.gameObject.CompareTag("chicken") && !_perceivedFood.Contains(col.gameObject)) {
                    _perceivedFood.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if (dist < closestFood) {
                        closestFood = dist;
                        foodTarget = col.gameObject;
                        return;
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
                if (col.gameObject.CompareTag("cat") && col.gameObject.GetComponent<Animal>().getIsFemale() != _animal.getIsFemale()
                    && !col.gameObject.GetComponent<Cat>().getIsBusy() && col.gameObject.GetComponent<Cat>().hasUrge
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

        if(isInDanger) {
            if(hunterTarget != null) {
                _animal.setTarget(hunterTarget);
            }
            _catStates = catStates.Evading;
            movementManager();
            return;
        }

        else if (isHungry && foodTarget != null /*&& !hasUrge*/) {

            _animal.setTarget(foodTarget);
            _catStates = catStates.Seeking;

            if (Vector3.Distance(transform.position, foodTarget.transform.position) <= 2f) {
                _catStates = catStates.Eating;

                actionManager();
                return;
            }
        }

        else if (isThirsty && waterTarget != null /*&& !hasUrge*/) {

            _animal.setTarget(waterTarget);
            _catStates = catStates.Seeking;

            if (Vector3.Distance(transform.position, waterTarget.transform.position) <= 2f) {
                _catStates = catStates.Drinking;

                actionManager();
                return;
            }
        }

        else if (hasUrge && partnerTarget != null) {
            _animal.setTarget(partnerTarget);
            _catStates = catStates.Seeking;

            if (Vector3.Distance(transform.position, partnerTarget.transform.position) <= 2f) {

                _catStates = catStates.Reproducing;
                actionManager();
                return;
            }
        }

        if (isSatisfied) {
            _catStates = catStates.Wandering;
        }

        movementManager();
        actionManager();
    }

    void movementManager() {

        switch (_catStates) {
            case catStates.None:
                _animal.ChangeAnimalState(AnimalState.Idle);
                break;
            case catStates.Wandering:
                _animal.ChangeAnimalState(AnimalState.Wander);
                break;
            case catStates.Seeking:
                _animal.ChangeAnimalState(AnimalState.Seek);
                break;
            case catStates.Evading:
                _animal.ChangeAnimalState(AnimalState.Evade);
                break;
            default: break;
        }
    }

    void actionManager() {
        switch (_catStates) {
            case catStates.Eating:
                _animal.ChangeAnimalState(AnimalState.Eat);
                eat(foodTarget);
                break;
            case catStates.Drinking:
                _animal.ChangeAnimalState(AnimalState.Eat);
                drink(waterTarget);
                // _perceivedWater.Clear();
                break;
            case catStates.Reproducing:
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
            _animal.procreate(t_partner.GetComponent<Animal>(), catPrefab);
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
