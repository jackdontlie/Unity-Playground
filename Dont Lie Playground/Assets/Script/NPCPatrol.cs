using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    //Dictate if agents wait on each nodes
    [SerializeField]
    bool _patrolWaiting;

    //Total time we wait at each node. eg. 3 Seconds
    [SerializeField]
    float _totalWaitTime = 3f;

    //Probabilty of changing minds. eg. 20%
    [SerializeField]
    float _switchProbability = 0.2f;

    //List of all Waypoints to visit.
    [SerializeField]
    List<Waypoint> _patrolPoints;

    //Private variables for base behaviours.
    private Animator _animator;
    NavMeshAgent _navMeshAgent;
    int _currentPatrolIndex;
    bool _travelling;
    bool _waiting;
    bool _patrolFoward;
    float _waitTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);

        }

        else
        {
            if (_patrolPoints != null && _patrolPoints.Count >= 2)
            {
                _currentPatrolIndex = 0;
                SetWaypointDistination();
            }

            else
            {
                Debug.LogError(" Need more waypoints for AI to patrol to! ");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if AI is close to destination
        if (_travelling && _navMeshAgent.remainingDistance <= 1.0f)
        {
            _travelling = false;

            //If we want AI to wait
            if (_patrolWaiting)
            {
                _waiting = true;
                _waitTimer = 0f;
                _animator.SetBool("Wait", true);
            }
            else
            {
                ChangePatrolPoint();
                SetWaypointDistination();
                _animator.SetBool("Wait", false);
            }
        }

        //Check if AI is waiting
        if (_waiting)
        {
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= _totalWaitTime)
            {
                _waiting = false;
                _animator.SetBool("Wait", false);
                ChangePatrolPoint();
                SetWaypointDistination();
            }
        }
    }

    private void SetWaypointDistination()
    {
        //Check if patrolpoint exist beforehand
        if (_patrolPoints != null)
        {
            Vector3 targetVector = _patrolPoints[_currentPatrolIndex].transform.position;
            _navMeshAgent.SetDestination(targetVector);
            _travelling = true;
        }
    }

    private void ChangePatrolPoint()
    {
        //Decide if AI switch back to previous waypoint
        if (Random.Range(0f, 1f) <= _switchProbability)
        {
            _patrolFoward = !_patrolFoward;
            
        }

        if (_patrolFoward)
        {
            _currentPatrolIndex++;

            if (_currentPatrolIndex >= _patrolPoints.Count)
            {
                _currentPatrolIndex = 0;
            }
        }

        else
        {
            _currentPatrolIndex--;

            if (_currentPatrolIndex < 0)
            {
                _currentPatrolIndex = _patrolPoints.Count - 1;
            }
        }
    }

}
