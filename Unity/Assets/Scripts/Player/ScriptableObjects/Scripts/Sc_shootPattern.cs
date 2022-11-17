using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShootPattern", menuName = "ScriptableObjects/Patterns/Shoot", order = 1)]
public class Sc_shootPattern : ScriptableObject
{
    public GameObject bulletPrefab;
    public GameObject rayPrefab;

    public bool randomOrder;
    int lastPattern = -1;

    [TableList(ShowIndexLabels = true)]
    public List<Pattern> Patterns = new List<Pattern>();
    
    public enum PatternType { Bullet, Ray };
    
    [System.Serializable]
    public class Pattern
    {
        [VerticalGroup("Type")]
        [HideLabel]
        [TableColumnWidth(80, resizable: false)]
        public PatternType MunitionState;

        [VerticalGroup("Type")]
        [HideLabel]
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Texture icon;


        //Bullets Variables
        //[ShowIf("PatternState", PatternType.Bullet), VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250)]
        //public int numberOfPreojectile, shootAngle, shootDamage;
        //Rays Variables
        [ShowIf("MunitionState", PatternType.Ray), VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250), GUIColor(1f,0.8f,0.8f)]
        public float RayDuration = 1, RaySize = 1;
        [ShowIf("MunitionState", PatternType.Ray), VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250), GUIColor(1f, 0.8f, 0.8f)]
        public bool rotate;
        [ShowIf("@this.rotate && this.MunitionState == PatternType.Ray"), VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250), GUIColor(1f, 0.8f, 0.8f)]
        //[ShowIf("MunitionState", PatternType.Ray), VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250), GUIColor(1f, 0.8f, 0.8f)]
        public float rotateSpeed = 1;

        //AllTypes Variables
        [VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250)]
        public int shootNumber, shootAngle, shootDamage;
        [VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250)]
        public bool AimPlayer = true;
        [VerticalGroup("Data"), LabelWidth(200), TableColumnWidth(250)]
        public float delayBeforeNextPattern;
    }

    [PropertySpace(SpaceBefore = 5, SpaceAfter = 15)]
    [System.Serializable]
    public class shootData
    {
        [PropertySpace(SpaceBefore = 5, SpaceAfter = 15)]
        public Sc_shootPattern shootPattern;
        
        [HideInInspector]
        public GameObject owner;

        [HideInInspector]
        public Sc_PoolsManager poolsManager;

        public void resetValues(GameObject refGo)
        {
            owner = refGo;
            poolsManager = GameObject.FindGameObjectWithTag("Pools").GetComponent<Sc_PoolsManager>();
        }
    }
    //rajouter le feedback de l'oeil qui clignote
    public IEnumerator shoot(shootData data)
    {
        foreach (Pattern pattern in Patterns)
        {
            Pattern p = pattern;

            if (randomOrder && Patterns.Count > 0)
            {
                bool next = false;
                while (!next)
                {
                    int x = (int)Random.Range(0, Patterns.Count);
                    if (x != lastPattern || Patterns.Count == 2)
                    {
                        p = Patterns[x];
                        next = true;

                        lastPattern = x;
                    }
                }
            }


            float bonusAngle = (p.shootAngle / p.shootNumber);
            int impaireDif = 0;
            if (p.shootNumber % 2 != 0 && p.shootNumber != 1)
            {
                impaireDif = 1;
            }
            else bonusAngle = (p.shootAngle / (p.shootNumber + 1));

            float gap = 0;
            
            for (int i = 0; i < p.shootNumber; i++)
            {

                if ((i + impaireDif) % 2 == 0)
                {
                    gap += bonusAngle;
                }
                gap *= -1;

                GameObject munition = null;
                switch (p.MunitionState)
                {
                    case (PatternType.Bullet):
                        munition = data.poolsManager.ballToSpawn(Sc_PoolsManager.poolType.Bullet);
                        break;
                    case (PatternType.Ray):
                        munition = data.poolsManager.ballToSpawn(Sc_PoolsManager.poolType.Ray);
                        break;
                }

                //GameObject munition = data.poolsManager.ballToSpawn(Sc_PoolsManager.poolType.Bullet);

                munition.GetComponent<Sc_Munition>().characterState = Sc_Munition.characterType.Enemy;
                munition.GetComponent<Sc_Munition>().damage = p.shootDamage;
                munition.GetComponent<Sc_Munition>().owner = data.owner;

                Vector2 direction = munition.GetComponent<Sc_Munition>().owner.GetComponent<Sc_enemyAI>().getVisorDirection();
                Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 + gap) * direction;
                munition.transform.rotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
                //
                Vector2 spawnPos = direction.normalized * 0.8f + (Vector2)munition.GetComponent<Sc_Munition>().owner.transform.position;
                munition.transform.position = spawnPos;

                if (p.AimPlayer)
                {

                }
                else
                {
                    munition.transform.position = data.owner.transform.position;
                    munition.transform.position += munition.transform.forward.normalized * 10;
                }

                munition.GetComponent<Sc_Munition>().WakeUp.Invoke();
                //munition.GetComponent<Sc_Bullet>().WakeUp();

                switch (p.MunitionState)
                {
                    case (PatternType.Bullet):
                        break;
                    case (PatternType.Ray):
                        munition.GetComponent<Sc_Munition>().speed = p.RayDuration;
                        if (p.rotate)
                        {
                            munition.GetComponent<Sc_Ray>().startCoroutineRotateAround(data.owner.transform, p.rotateSpeed);
                        }
                        break;
                }
            }

            yield return new WaitForSeconds(p.delayBeforeNextPattern);
        }

        data.owner.GetComponent<Sc_enemyAI>().canShoot = true;
    }
}
