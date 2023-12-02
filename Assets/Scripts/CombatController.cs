using UnityEngine;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{

    [Header("Estadísticas de la unidad")]
    public int startHP;
    private int currentHP;
    public int attackDmg;
    public int attackSpeed;
    public int attackRange;
    public bool shooter;
    public GameObject bulletPrefab;
    public bool isPlayerUnit;
    private float attackCooldown;
    public bool isBeingDeployed;

    [Header("UI")]
    public Canvas healthCanvas;
    public Image healthBar;

    private void Start()
    {
        currentHP = startHP;
        healthCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingDeployed)
        {
            return;
        }

        if (currentHP <= 0)
        {
            resetColor();
            Destroy(gameObject);
        }

        if (attackCooldown <= attackSpeed)
        {
            attackCooldown += Time.deltaTime;
        }
    }

    //Realiza un ataque a la unidad que recibe como objetivo
    public void attack(GameObject target)
    {
        if (target != null && attackCooldown >= attackSpeed)
        {
            if (shooter)
            {
                GameObject newBullet = Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
                newBullet.GetComponent<BulletController>().setTarget(target);
                newBullet.GetComponent<BulletController>().setDmg(attackDmg);
            }
            else
            {
                target.GetComponent<CombatController>().receiveDmg(attackDmg);
            }
            attackCooldown = 0;
            if (!isPlayerUnit)
            {
                this.GetComponent<EnemyController>().currentState = 1;
            }
            else
            {
                //TODO: Cuando la unidad aliada ataca a la unidad enemiga
            }
            CancelInvoke("resetColor");
        }
    }

    //Recibe un ataque, por lo que resta la vida de la unidad y cambia el color del sprite para reflejarlo
    public void receiveDmg(int dmg)
    {
        currentHP -= dmg;
        healthCanvas.enabled = true;
        healthBar.fillAmount = currentHP / (float)startHP;
        this.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        Invoke("resetColor", 0.3f);
    }

    private void resetColor()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
    }
}