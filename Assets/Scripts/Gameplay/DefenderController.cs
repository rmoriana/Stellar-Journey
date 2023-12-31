using UnityEngine;
using UnityEngine.AI;

public class DefenderController : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshAgent agent;
    public GameObject currentTarget;
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<CombatController>().isBeingDeployed)
        {
            return;
        }

        if (currentTarget != null && checkIfTargetIsInRange() && checkIfTargetIsVisible(currentTarget))
        {
            GetComponent<CombatController>().attack(currentTarget);
        }
        else
        {
            currentTarget = findNearestVisibleEnemy();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, GetComponent<CombatController>().attackRange);
    }

    private GameObject findNearestVisibleEnemy()
    {
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("EnemyUnit");
        if (enemyArray.Length > 0)
        {
            GameObject nearestEnemy = null;
            float distance = GetComponent<CombatController>().attackRange;
            for (int i = 0; i < enemyArray.Length; i++)
            {

                if (distance > Vector2.Distance(transform.position, enemyArray[i].transform.position))
                {
                    if (checkIfTargetIsVisible(enemyArray[i]))
                    {
                        nearestEnemy = enemyArray[i];
                        distance = Vector2.Distance(transform.position, nearestEnemy.transform.position);
                    }
                }
            }
            return nearestEnemy;
        }
        return null;
    }

    private bool checkIfTargetIsVisible(GameObject target)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Pared"); //Así detecta solo esta capa
        RaycastHit2D hit = Physics2D.Linecast(new Vector2(transform.position.x, transform.position.y + 0.5f), target.transform.position, layerMask);
        //Debug.DrawLine(transform.position, target.transform.position, Color.blue);
        if (hit != false && hit.distance < Vector2.Distance(transform.position, target.transform.position))
        {
            return false;
        }
        return true;
    }

    private bool checkIfTargetIsInRange()
    {
        if (Vector2.Distance(transform.position, currentTarget.transform.position) < GetComponent<CombatController>().attackRange)
        {
            return true;
        }

        return false;
    }
}
