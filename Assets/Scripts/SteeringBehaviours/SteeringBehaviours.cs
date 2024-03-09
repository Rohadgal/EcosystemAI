using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides steering behaviors for agents, such as seeking, fleeing, pursuit, and more.
/// </summary>
public class SteeringBehaviours : MonoBehaviour
{


    /// <summary>
    /// Slows down the agent on arrival to the target or accelerates the agent's flee speed.
    /// </summary>
    /// <param name="t_animal">The agent to slow down or speed up.</param>
    /// <param name="target">The target position.</param>
    /// <param name="desiredVel">The desired velocity.</param>
    /// <returns>The adjusted desired velocity.</returns>
    private static Vector3 arrival(Animal t_animal, Vector3 target, Vector3 desiredVel) {

        float buffer = 3f;
        float distance = Vector3.Distance(t_animal.transform.position, target);
        if (distance + buffer <= t_animal.getSlowingRadius()) {
            //desiredVel.Normalize();
            desiredVel *= (distance / t_animal.getSlowingRadius());
        }
        return desiredVel;
    }

    /// <summary>
    /// Sets the steering force of the agent using the desired velocity.
    /// </summary>
    /// <param name="t_animal">The agent to apply the steering force.</param>
    /// <param name="desiredVel">The desired velocity.</param>
    private static void addSteringForce(Animal t_animal, Vector3 desiredVel) {
        Vector3 steeringForce = desiredVel - t_animal.rb.velocity;
        steeringForce = Vector3.ClampMagnitude(steeringForce, t_animal.getMaxForce());
        steeringForce /= t_animal.rb.mass;
        t_animal.rb.velocity = Vector3.ClampMagnitude(t_animal.rb.velocity + new Vector3(steeringForce.x, 0, steeringForce.z), t_animal.getMaxSpeed());
       
        FaceForwardDirection(t_animal);
    }

    /// <summary>
    /// Changes the current velocity of the agent to seek a given target.
    /// </summary>
    /// <param name="t_animal">The agent to perform seeking.</param>
    /// <param name="t_target">The target position to seek.</param>
    public static void seek(Animal t_animal, Vector3 t_target) {
        Vector3 desiredVel = t_target - t_animal.transform.position;
        desiredVel.Normalize();
        desiredVel *= t_animal.getMaxSpeed();
        desiredVel = arrival(t_animal, t_target, desiredVel);
        addSteringForce(t_animal, desiredVel);
    }

    /// <summary>
    /// Changes the current velocity of the agent to flee from the target.
    /// </summary>
    /// <param name="t_animal">The agent to perform fleeing.</param>
    /// <param name="t_target">The target position from which to flee.</param>
    public static void flee(Animal t_animal, Vector3 t_target) {
        Vector3 desiredVel = t_animal.transform.position - t_target;
        desiredVel.Normalize();
        desiredVel *= t_animal.getMaxSpeed();
        addSteringForce(t_animal, desiredVel);
    }
    /// <summary>
    /// Makes the agent pursue a target by predicting where the target's position will be in the future.
    /// </summary>
    /// <param name="t_animal">The agent performing pursuit.</param>
    /// <param name="t_target">The target to be chased.</param>
    public static void pursuit(Animal t_animal, Animal t_target) {
        Vector3 futurePos = predictTargetPos(t_animal, t_target);
        seek(t_animal, futurePos);
    }
    /// <summary>
    /// Makes the agent evade a specific target.
    /// </summary>
    /// <param name="t_animal">The agent performing evasion.</param>
    /// <param name="t_target">The target to be evaded.</param>
    public static void evade(Animal t_animal, Animal t_target) {
        Vector3 futurePos = predictTargetPos(t_animal, t_target);
        flee(t_animal, futurePos);
    }
    /// <summary>
    /// Predicts the future forward position of a target based on its velocity.
    /// </summary>
    /// <param name="t_animal">The agent predicting the target's position.</param>
    /// <param name="t_target">The target to make a prediction about.</param>
    /// <returns>The predicted future position of the target.</returns>
    public static Vector3 predictTargetPos(Animal t_animal, Animal t_target) {
        Vector3 targetDistance = t_target.getPos() - t_animal.transform.position;
        targetDistance.y = 0;
        float T = (targetDistance.magnitude) / t_animal.getMaxSpeed();
        Vector3 velocity = (t_target.rb.velocity == Vector3.zero) ? Vector3.one : t_animal.rb.velocity;
        Vector3 futurePos = t_target.getPos() + (velocity * T);
        futurePos.y = 0;
        return futurePos;
    }
    /// <summary>
    /// Slows down the agent on arrival to the target or accelerates the agent's flee speed.
    /// </summary>
    /// <param name="t_animal">The agent to slow down or speed up.</param>
    /// <param name="target">The target position.</param>
    /// <param name="desiredVel">The desired velocity.</param>
    public static void wander(Animal t_animal, float t_circleDistance, float t_circleRadius, float t_angleChange) {
        Vector3 circleCenter = t_animal.rb.velocity.normalized;
        circleCenter *= t_circleDistance;
        float min = 0f, max = 6.2f;
        Vector3 displacement = new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max)).normalized;
        displacement *= t_circleRadius;
        // Change angle just a bit, so it 
        // won't have the same value in the 
        // next game frame.
        float angle = Random.Range(0f, 360f);
        angle += (Random.Range(0f, 1f) * t_angleChange) - (t_angleChange * 0.5f);
        Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * displacement;
        Vector3 wanderForce = circleCenter + displacement;
        seek(t_animal, wanderForce);
    }

    /**
    * @brief Function that makes the agent face forward.
*/
    static void FaceForwardDirection(Animal t_animal) {
        // Get the direction the GameObject is moving (based on its current velocity)
        Vector3 forwardDirection = t_animal.rb.velocity.normalized;
        // Make the GameObject look towards the calculated forward direction
        if (forwardDirection != Vector3.zero) {
            t_animal.transform.LookAt(t_animal.transform.position + forwardDirection);
        }
    }
}
