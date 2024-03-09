using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum AnimalState { None, Idle, Eat, Seek, Pursuit, Evade, Wander }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent (typeof(Animator))]
public class Animal : MonoBehaviour
{
    #region Steering Behaviour Attributes
    [SerializeField]
    float m_maxSpeed = 20f, m_maxForce = 10f, m_slowingRadius;
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
    //[SerializeField]
    //Image m_reproductiveUrgeBarSprite;
    //[SerializeField]
    //GameObject m_reproductiveUrgeGameObject;
    private Camera m_camera;
    #endregion

    public Rigidbody rb;

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
            default: break;
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
        SteeringBehaviours.wander(this, 550, 15, 0);
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
                m_animator.SetBool("isEating", true);
                break;
            case AnimalState.Wander:
                m_animator.SetBool("isWandering", true);
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

    public void updateHealthBar(float maxHealthBar, float currentHealth) {
        //if (m_reproductiveUrgeBarSprite != null) {
        //    m_reproductiveUrgeBarSprite.fillAmount = currentHealth / maxHealthBar;
        //}
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, m_perceptionRadius);
    }
}