using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum AnimalState { None, Idle, Eat, Seek, Pursuit, Evade, Wander, Reproduce }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent (typeof(Animator))]
public class Animal : MonoBehaviour
{
    #region Steering Behaviour Attributes
   // [SerializeField]
    float m_maxSpeed = 5f, m_maxForce = 12f, m_slowingRadius;
    #endregion

    #region Perception
    [SerializeField]
    public float m_perceptionRadius = 10f;
    GameObject m_target;
    #endregion

    AnimalState state;
    [SerializeField]
    Animator m_animator;

    #region UI
    [SerializeField]
    Image m_reproductiveUrgeBarSprite, m_hungerBarSprite, m_thirstBarSprite;
    [SerializeField]
    GameObject m_needsCanvasGameObject;
    private Camera m_camera;
    #endregion

    #region Animal Characteristics
    bool isFemale;
    float _hunger, _thirst, _urge, _maxLevel = 100f;
    public Rigidbody rb;
    public Gene _gene;
    #endregion

    public struct Gene {
        public float feelHungry { get; set; }
        public float feelThirst { get; set; }
        public float feelUrge { get; set;}

        public void hungerSystem(Animal t_animal, float t_increment) {
            float currentHungerLevel = t_animal.getHunger() + t_increment;
            t_animal.setHunger(currentHungerLevel);
            t_animal.updateHungerBar(t_animal.getHunger());
        }

        public void thirstSystem(Animal t_animal, float t_increment) {
            float currentThirstLevel = t_animal.getThirst() + t_increment;
            t_animal.setThirst(currentThirstLevel);
            t_animal.updateThirstBar(t_animal.getThirst());
        }

        public void urgeSystem(Animal t_animal, float t_increment) {
            float currentUrgeLevel = t_animal.getUrge() + t_increment;
            t_animal.setUrge(currentUrgeLevel);
            t_animal.updateUrgeBar(t_animal.getUrge());
        }
    }

    private void Awake() {
        _gene = new Gene();
        setMaxSpeed(Random.Range(3f, 9f));
        setMaxForce(Random.Range(4f, 12f));
        setPerceptionRadius(Random.Range(10f, 16f));
    }

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        rb = GetComponent<Rigidbody>();
        state = AnimalState.None;
    }

    private void Update() {
        switch (state) {
            case AnimalState.None: break;
            case AnimalState.Idle: break;
            case AnimalState.Seek: seek(); break;
            case AnimalState.Pursuit: pursuit(); break;
            case AnimalState.Evade: evade(); break;
            case AnimalState.Wander: wander(); break;
            case AnimalState.Reproduce: break;
            default: break;
        }

        if(m_needsCanvasGameObject != null) {
            m_needsCanvasGameObject.transform.rotation = Quaternion.LookRotation(m_needsCanvasGameObject.transform.position - m_camera.transform.position);
        }
    }

    #region Steering Behaviours
    void seek() {
        SteeringBehaviours.seek(this, new Vector3(m_target.gameObject.transform.position.x, this.transform.position.y, m_target.gameObject.transform.position.z));
    }
    /// <summary>
    /// Steering behavior for the "Fleeing" state.
    /// </summary>
    void flee() {
        SteeringBehaviours.flee(this, new Vector3(m_target.gameObject.transform.position.x, this.transform.position.y, m_target.gameObject.transform.position.z));
    }
    /// <summary>
    /// Steering behavior for the "Pursuiting" state.
    /// </summary>
    void pursuit() {
        SteeringBehaviours.pursuit(this, m_target.GetComponent<Animal>());
    }
    /// <summary>
    /// Steering behavior for the "Evading" state.
    /// </summary>
    void evade() {
        SteeringBehaviours.evade(this, m_target.GetComponent<Animal>());
    }
    /// <summary>
    /// Steering behavior for the "Wandering" state.
    /// </summary>
    void wander() {
        SteeringBehaviours.wander(this, 280, 150, 90);
    }

    /// <summary>
    /// Steering behavior for the "Attacking" state.
    /// </summary>
    void attack() {

    }
    /// <summary>
    /// Steering behavior for the "Dead" state.
    /// </summary>
    void die() {
        //isAlive = false;
        rb.velocity = Vector3.zero;
        m_maxSpeed = 0;
        rb.freezeRotation = true;
        StartCoroutine(deactivate());
    }
    #endregion

    #region Getters
    /// <summary>
    /// Gets the slowing radius for the agent.
    /// </summary>
    public float getSlowingRadius() { return m_slowingRadius; }
    /// <summary>
    /// Gets the perception radius for the agent.
    /// </summary>
    public float getPerceptionRadius() { return m_perceptionRadius; }
    /// <summary>
    /// Gets the maximum speed of the agent.
    /// </summary>
    public float getMaxSpeed() { return m_maxSpeed; }
    /// <summary>
    /// Gets the maximum force for steering behaviors.
    /// </summary>
    public float getMaxForce() { return m_maxForce; }
    /// <summary>
    /// Gets the position of the agent.
    /// </summary>
    public Vector3 getPos() { return this.transform.position; }
    /// <summary>
    /// Gets the current state of the agent.
    /// </summary>
    public AnimalState getAgentState() { return state; }
    /// <summary>
    /// Gets the animator component.
    /// </summary>
    public Animator getAnimator() { return m_animator; }
    /// <summary>
    /// Gets the target of the agent.
    /// </summary>
    public GameObject getTarget() { return m_target; }

    public bool getIsFemale() { return isFemale; }

    public float getHunger() { return _hunger; }

    public float getThirst() { return _thirst; }

    public float getUrge() { return _urge; }
    #endregion

    #region Setters
    /// <summary>
    /// Changes the state of the agent to a new state.
    /// </summary>
    public void ChangeAnimalState(AnimalState newState) {
        if (state == newState) {

            return;
        }
        resetAnimatorParameters();

        state = newState;
        switch (state) {
            case AnimalState.None:
                break;
            case AnimalState.Idle:
                m_animator.SetBool("isIdle", true);
                rb.velocity = Vector3.zero;
                break;
            case AnimalState.Seek:
                m_animator.SetBool("isSeeking", true);
                break;
            case AnimalState.Pursuit:
                m_animator.SetBool("isPursuiting", true);
                break;
            case AnimalState.Evade:
                m_animator.SetBool("isEvading", true);
                break;
            case AnimalState.Eat:
                m_animator.SetBool("isIdle", true);
                rb.velocity = Vector3.zero;
                break;
            case AnimalState.Wander:
                m_animator.SetBool("isWandering", true);
                break;
            case AnimalState.Reproduce:
                m_animator.SetBool("isIdle", true);
                rb.velocity = Vector3.zero;
                break;
            default: break;
        }
    }

    /// <summary>
    /// Resets animator parameters to their default values.
    /// </summary>
    private void resetAnimatorParameters() {
        // rb.constraints &= ~RigidbodyConstraints.FreezeRotationY; 

        foreach (AnimatorControllerParameter parameter in m_animator.parameters) {
            if (parameter.type == AnimatorControllerParameterType.Bool) {
                m_animator.SetBool(parameter.name, false);
            }
        }
    }

    /// <summary>
    /// Sets the target of the agent.
    /// </summary>
    public void setTarget(GameObject t_target) {
        m_target = t_target;
    }

    /// <summary>
    /// Sets the maximum speed of the agent.
    /// </summary>
    public void setMaxSpeed(float t_maxSpeed) {
        m_maxSpeed = t_maxSpeed;
    }

    public void setMaxForce(float t_maxForce) {
        m_maxForce = t_maxForce;
    }

    public void setGender(bool t_isFemale) {
        isFemale = t_isFemale;
    }

    public void setHunger(float t_hunger) { 
        _hunger = t_hunger;
    }

    public void setThirst(float t_thirst) {
        _thirst = t_thirst;
    }

    public void setUrge(float t_urge) {
        _urge = t_urge;
    }


    public void updateHungerBar(float currentHunger) {
        if(m_hungerBarSprite != null) {
            m_hungerBarSprite.fillAmount = currentHunger / _maxLevel;
        }
    }

    public void updateThirstBar(float currentThirst) {
        if(m_thirstBarSprite != null) {
            m_thirstBarSprite.fillAmount = currentThirst / _maxLevel;
        }
    }

    public void updateUrgeBar(float currentUrge) {
        if(m_reproductiveUrgeBarSprite != null) {
            m_reproductiveUrgeBarSprite.fillAmount= currentUrge / _maxLevel;
        }
    }

    public void setPerceptionRadius(float radius) {
        m_perceptionRadius = radius;
    }

    /// <summary>
    /// Coroutine to deactivate the object after a certain time.
    /// </summary>
    IEnumerator deactivate() {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    #endregion

    void OnDrawGizmos() {
        if(m_target != null) {

            Gizmos.color = Color.red;
           // Gizmos.DrawWireSphere(this.transform.position, m_perceptionRadius);
            //Gizmos.color = Color.cyan;
            Gizmos.DrawLine(this.transform.position, m_target.transform.position);
        }
    }

    Gene GenerateOffspringGene(Gene partnerGene) {
        Gene offspringGene = new Gene();
       
        //if(Random.Range(0,100) < 30) {
        //    randomValue = Random.Range(0.2f,1.2f);
        //}
        offspringGene.feelHungry = (_gene.feelHungry + partnerGene.feelHungry) * 0.5f;
        offspringGene.feelThirst = (_gene.feelThirst + partnerGene.feelThirst) * 0.5f;
        offspringGene.feelUrge = (_gene.feelUrge + partnerGene.feelUrge) * 0.5f;

        return offspringGene;
    }

    public void procreate(Animal t_partner, GameObject t_prefab) {
        Gene offspringGene = GenerateOffspringGene(t_partner._gene);

  

        Animal offspringOne = instantiateOffspring(t_prefab);
        Animal offspringTwo = instantiateOffspring(t_prefab);

        float randomValue = Random.Range(1.2f, 2f);

        float speed = ((t_partner.getMaxSpeed() + m_maxSpeed) * 0.5f);
        float perceptionRadius = ((t_partner.getPerceptionRadius() + m_perceptionRadius) * 0.5f);

        if(Random.Range(0, 100) < 30) {
            speed *= randomValue;
            perceptionRadius *= randomValue;
        }

        offspringOne.setMaxSpeed(speed);
        offspringOne.setPerceptionRadius(perceptionRadius);

        offspringTwo.setMaxSpeed(speed);
        offspringTwo.setPerceptionRadius(perceptionRadius);

        offspringOne._gene = offspringGene;
        offspringTwo._gene = offspringGene;
    }

    public Animal instantiateOffspring(GameObject t_animalTypePrefab) {
        GameObject offspring = Instantiate(t_animalTypePrefab, transform.position, Quaternion.identity);

        Animal offspringAnimal = offspring.GetComponent<Animal>();

        return offspringAnimal;
    }
}
