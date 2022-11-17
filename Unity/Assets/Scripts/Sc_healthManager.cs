using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine;

public class Sc_healthManager : MonoBehaviour
{
    [HideInInspector]
    public enum characterType { Player, Enemy };
    public characterType characterState;

    public delegate void healthEvent();
    public List<healthEvent> OnHitEvents = new List<healthEvent>();
    public List<healthEvent> OnDeathEvents = new List<healthEvent>();

    [ShowIf("characterState",characterType.Player)]
    public Image healthBar;

    Rigidbody2D rb;
    [HideInInspector]
    public Vector2 hitPoint;

    public int healthPoint;
    [HideInInspector]
    public int maxHealthPoint;

    [HideInInspector]
    public bool isInvulnerable;

    SpriteRenderer graphic;
    Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;

    private void OnDrawGizmos()
    {
        if (gameObject.tag == "Player")
        {
            characterState = characterType.Player;
        }
        else if (gameObject.tag == "Enemy")
        {
            characterState = characterType.Enemy;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealthPoint = healthPoint;
        
        graphic = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        switch (characterState)
        {
            case (characterType.Player):
                OnDeathEvents.Add(HealthReset);
                break;

            case (characterType.Enemy):
                gradient = new Gradient();
                // Populate the color keys at the relative time 0 and 1 (0 and 100%)
                colorKey = new GradientColorKey[2];
                colorKey[0].color = Color.gray;
                colorKey[0].time = 0.0f;
                colorKey[1].color = Color.red;
                colorKey[1].time = 1.0f;
                // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
                alphaKey = new GradientAlphaKey[2];
                alphaKey[0].alpha = 0.66f;
                alphaKey[0].time = 0.0f;
                alphaKey[1].alpha = 1.0f;
                alphaKey[1].time = 1.0f;
                // set gradient keys
                gradient.SetKeys(colorKey, alphaKey);

                break;
        }

        //OnDeathEvents.Add(HealthReset);

        WakeUp();
    }

    public void WakeUp()
    {
        switch (characterState)
        {
            case (characterType.Player):
                if (maxHealthPoint > 0) healthBar.fillAmount = (float)healthPoint / (float)maxHealthPoint;
                break;

            case (characterType.Enemy):
                if (maxHealthPoint > 0) graphic.color = gradient.Evaluate((float)healthPoint / (float)maxHealthPoint);
                break;
        }
    }
    public void aMimir()
    {
        Events(OnDeathEvents);     

        switch (characterState)
        {
            case (characterType.Player):
                //Events(OnDeathEvents);
                break;

            case (characterType.Enemy):
                //GetComponent<Sc_enemyAI>().isActive = false;
                //GetComponent<Rigidbody2D>().gravityScale = 1;
                //GetComponent<Rigidbody2D>().AddForce(Vector2.right * -1000);
                //gameObject.SetActive(false);
                break;
        }
    }

    public void HealthReset()
    {
        healthPoint = maxHealthPoint;
        WakeUp();
    }

    public void takeDamage(int damage)
    {
        if (!isInvulnerable && healthPoint > 0)
        {
            healthPoint -= damage;

            switch (characterState)
            {
                case (characterType.Player):
                    healthBar.fillAmount = (float)healthPoint / (float)maxHealthPoint;
                    StartCoroutine(isInvulnerableDelay());
                    break;

                case (characterType.Enemy):
                    if(healthPoint > 0) graphic.color = gradient.Evaluate((float)healthPoint / (float)maxHealthPoint);
                    break;
            }

            if (healthPoint <= 0)
            {
                healthPoint = 0;

                aMimir();
            }
            else
            {
                if(characterState == characterType.Player) takePush((Vector2)transform.position - hitPoint, false);
            }
        }
    }
    [ShowIf("characterState", characterType.Player)]
    public float invulnerabilityDelay = 1f;
    IEnumerator isInvulnerableDelay()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDelay);
        isInvulnerable = false;
    }

    public void takePush(Vector2 dir, bool powerfulPush)
    {
        float power = 40;
        if (powerfulPush) power *= 30;

        rb.AddForce(dir.normalized * power); //* rb.velocity.magnitude);
    }


    public void takeHeal(int heal, bool overdrive)
    {
        int tempoHealth = healthPoint + heal;

        if (!overdrive)
        {
            if (tempoHealth > maxHealthPoint)
            {
                healthPoint = maxHealthPoint;
            }
            else healthPoint = tempoHealth;
        }
        else
        {
            if (tempoHealth > maxHealthPoint * 2)
            {
                healthPoint = maxHealthPoint * 2;
            }
            else healthPoint = tempoHealth;
        }

        healthBar.fillAmount = (float)healthPoint / (float)maxHealthPoint;
    }

    public void takeHit()
    {
        switch (characterState)
        {
            case (characterType.Player):
                Events(OnHitEvents);
                break;

            case (characterType.Enemy):
                Events(OnHitEvents);
                break;
        }
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        switch (characterState)
        {
            case (characterType.Player):
                if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "Spike")
                {
                    hitPoint = col.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position);
                    takeDamage(5);

                    takePush((Vector2)transform.position - hitPoint, true);
                }
                break;

                //case (characterType.Enemy):
                //    graphic.color = gradient.Evaluate((float)healthPoint / (float)maxHealthPoint);
                //    break;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (characterState)
        {
            case (characterType.Player):
                if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Spike")
                {
                    hitPoint = other.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position);
                    takeDamage(5);

                    takePush((Vector2)transform.position - hitPoint, true);
                }
                break;

                //case (characterType.Enemy):
                //    graphic.color = gradient.Evaluate((float)healthPoint / (float)maxHealthPoint);
                //    break;
        }
    }


    private void Events(List<healthEvent> events)
    {
        if (this.enabled)
        {
            for (int i = 0; i < events.Count; i++)
            {
                events[i]();
            }
        }
    }
}
