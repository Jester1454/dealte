using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

namespace Player.Behaviours.HealthSystem
{
    [RequireComponent(typeof(HealthBehaviour), typeof(GraphOwner))]
    public class StopBehaviourTreeOnDeath : MonoBehaviour
    {
        private HealthBehaviour _healthBehaviour;
        private void OnEnable()
        {
            _healthBehaviour = GetComponent<HealthBehaviour>();
            _healthBehaviour.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            var graphOwner = GetComponent<GraphOwner>();
            
            if (graphOwner != null)
            {
                graphOwner.PauseBehaviour();
                graphOwner.StopBehaviour();   
            }

            var agent = GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }

            var rigid = GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.detectCollisions = false;
            }

            var coll = GetComponent<Collider>();
            if (coll != null)
            {
                coll.enabled = false;
            }
        }

        private void OnDisable()
        {
            _healthBehaviour.OnDeath -= OnDeath;
        }
    }
}