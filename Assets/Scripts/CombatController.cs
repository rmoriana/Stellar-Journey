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
    public bool isSpaceship;

    [Header("UI")]
    public Canvas healthCanvas;
    public Image healthBar;

    private float resetColorTime;
    private float resetColorTimer;
    private bool colorChanged;

    private void Start()
    {
        resetColorTime = 0.3f;
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
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            healthCanvas.enabled = false;
            if (isSpaceship)
            {
                if (!GetComponent<Spaceship_C>().getGameHasFinished())
                {
                    GetComponent<Spaceship_C>().startEndGameSequence();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (attackCooldown <= attackSpeed)
        {
            attackCooldown += Time.deltaTime;
        }

        if (colorChanged)
        {
            resetColorTimer += Time.deltaTime;
            if(resetColorTimer > resetColorTime)
            {
                GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    //Realiza un ataque a la unidad que recibe como objetivo
    public void attack(GameObject target)
    {
        if (target != null && attackCooldown >= attackSpeed)
        {
            if (shooter)
            {
                GameObject newBullet = Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y+0.5f, 0), new Quaternion(0, 0, 0, 0));
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
                GetComponent<EnemyController>().currentState = 1;
            }
            else
            {
                //TODO: Cuando la unidad aliada ataca a la unidad enemiga (sin usar un proyectil)
            }
        }
    }

    //Recibe un ataque, por lo que resta la vida de la unidad y cambia el color del sprite para reflejarlo
    public void receiveDmg(int dmg)
    {
        if(isSpaceship && GetComponent<Spaceship_C>().getGameHasFinished())
        {
            return;
        }

        healthCanvas.enabled = true;
        currentHP -= dmg;
        healthBar.fillAmount = currentHP / (float)startHP;
        GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        colorChanged = true;
        resetColorTimer = 0;
    }
}