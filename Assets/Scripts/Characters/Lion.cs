using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum lionStates { None, Wandering, Seeking, Drinking, Eating, Reproducing, Evading, Dead }

public class Lion : MonoBehaviour
{
    Animal _animal;
    lionStates _lionStates;
    public List<GameObject> _perceivedFood, _perceivedWater, _perceivedPartner;
    bool isHungry = false, isThirsty = false, hasUrge = false,/* isBusy = false,*/ isSatisfied = true;
    GameObject foodTarget, waterTarget, partnerTarget;
    float closestPartner = Mathf.Infinity;
    float hungerIncrement = 0.23f, thirstIncrement = 0.25f, urgeIncrement = 0.3f;
    [SerializeField]
    GameObject lionPrefab;
    bool doCoroutine = true;

    public float raycastDistance = 3f;
    public LayerMask obstacleLayer;

    public float rotationSpeed = 120f;

    // Start is called before the first frame update
    void Start()
    {
        _animal = GetComponent<Animal>();
        _perceivedFood = new List<GameObject>();
        _perceivedWater = new List<GameObject>();
        _perceivedPartner = new List<GameObject>();
        _lionStates = lionStates.Wandering;

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
            if (hit.collider.gameObject != waterTarget || isHungry || hasUrge) {
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
        if (doCoroutine) {
            StartCoroutine(perceive());
        }
    }

    void perceptionManager() {
        Collider[] perceivedObjects = Physics.OverlapSphere(_animal.getPos(), _animal.getPerceptionRadius());

        if(perceivedObjects != null && perceivedObjects.Length > 0 ) {
            if (isHungry && !foodTarget) {
                seekFood(perceivedObjects);
            }
            else if (isThirsty && !waterTarget) {
                seekWater(perceivedObjects);
            }
            else if (hasUrge && !partnerTarget) {
                seekPartner(perceivedObjects);
            }
        }
        decisionManager();
    }

    void seekFood(Collider[] t_perceivedObjects) {
            foreach (Collider col in t_perceivedObjects) {
                if (col.gameObject.CompareTag("dog") && !_perceivedFood.Contains(col.gameObject)) {
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
                if (col.gameObject.CompareTag("lion") && col.gameObject.GetComponent<Animal>().getIsFemale() != _animal.getIsFemale()
                  && col.gameObject.GetComponent<Lion>().hasUrge
                    && !_perceivedPartner.Contains(col.gameObject)) {
                    _perceivedPartner.Add(col.gameObject);
                    float dist = Vector3.Distance(transform.position, col.gameObject.transform.position);
                    if (dist < closestPartner) {
                        closestPartner = dist;
                        partnerTarget = col.gameObject;
                        return;
                    }
                }
            }
        }
    }

    void decisionManager() {

        if (isHungry && foodTarget != null) {

            if(!foodTarget.activeSelf) {
                foodTarget = null;
                _perceivedFood.Clear();
                perceptionManager();
                return;
            }

            _animal.setTarget(foodTarget);
            _lionStates = lionStates.Seeking;

            if (Vector3.Distance(transform.position, foodTarget.transform.position) <= 3f) {
               _lionStates = lionStates.Eating;

                actionManager();
                return;
            }
            movementManager();
            return;
        }

        else if (isThirsty && waterTarget != null) {

            _animal.setTarget(waterTarget);
            _lionStates = lionStates.Seeking;

            if (Vector3.Distance(transform.position, waterTarget.transform.position) <= 3f) {
                _lionStates = lionStates.Drinking;

                actionManager();
                return;
            }
            movementManager();
            return;
        }

        else if (hasUrge && partnerTarget != null) {
            _animal.setTarget(partnerTarget);
            _lionStates = lionStates.Seeking;

            if (Vector3.Distance(transform.position, partnerTarget.transform.position) <= 4f) {
                _lionStates = lionStates.Reproducing;
                actionManager();
                return;
            }
            movementManager();
            return;
        }

        if (isSatisfied) {
            _lionStates = lionStates.Wandering;
        }

        movementManager();
    }

    void movementManager() {

        switch (_lionStates) {
            case lionStates.None:
                _animal.ChangeAnimalState(AnimalState.Idle);
                break;
            case lionStates.Wandering:
                _animal.ChangeAnimalState(AnimalState.Wander);
                break;
            case lionStates.Seeking:
                _animal.ChangeAnimalState(AnimalState.Seek);
                break;
            default: break;
        }
    }

    void actionManager() {
        switch (_lionStates) {
            case lionStates.Eating:
                _animal.ChangeAnimalState(AnimalState.Eat);
                eat(foodTarget);
                break;
            case lionStates.Drinking:
                _animal.ChangeAnimalState(AnimalState.Eat);
                drink(waterTarget);
                break;
            case lionStates.Reproducing:
                _animal.ChangeAnimalState(AnimalState.Reproduce);
                if(partnerTarget != null) {
                    reproduce(partnerTarget);
                }
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
       // Debug.Log(_animal.getIsFemale());
        _animal._gene.feelHungry = 20f;
        _animal._gene.feelThirst = 40f;
        _animal._gene.feelUrge = 60f;
    }

    void eat(GameObject _food) {
     
        isHungry = false;
        _food.SetActive(false);
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
        //if (!isHungry) {
        //    _animal.setTarget(null);
        //}
        _animal.setThirst(0f);
        isSatisfied = true;
    }

    void reproduce(GameObject t_partner) {
        if (_animal.getIsFemale()) {
            _animal.procreate(t_partner.GetComponent<Animal>(), lionPrefab);
        }
       // isBusy = false;
        _perceivedPartner.Clear();
        _animal.setTarget(null);
        partnerTarget = null;
        _animal.setUrge(0f);
        hasUrge = false;
    }

  //  public bool getIsBusy() { return isBusy; }

    public void setUrge(bool t_hasUrge) {
        hasUrge = t_hasUrge;
    }
}
