using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Pathfinding;

[CreateAssetMenu(fileName = "MovementPattern", menuName = "ScriptableObjects/Patterns/Movement", order = 2)]
public class Sc_movementPattern : ScriptableObject
{
    public float inCamLimit = 0.95f;
    public float playerDistanceLimit = 4f;

    public enum TargetType { ChasePlayer, Targets, Stationary, Other };
    public TargetType TargetState;

    public enum PlayerInSightType { Chase, Stop, Ignore };
    public PlayerInSightType PlayerInSightState;

    public void DrawPatternGizmo(List<Transform> targets, Transform refTr)
    {
        if (targets.Count == 0 && PlayerInSightState != PlayerInSightType.Ignore && TargetState != Sc_movementPattern.TargetType.Targets) targets.Add(GameObject.FindGameObjectWithTag("Player").transform);

        switch (TargetState)
        {
            case TargetType.Stationary:
                DrawGizmoPlayer(targets[0], refTr);
                break;
            case TargetType.Targets:
                DrawGizmoTargets(targets, refTr);

                if(targets.Count == 1 && targets[0] != null && targets[0].childCount > 0)
                {
                    Transform parent = targets[0];
                    targets.Clear();
                    for(int i = 0; i < parent.childCount; i++)
                    {
                        targets.Add(parent.GetChild(i));
                    }
                }
                break;
        }
    }

    private void DrawGizmoPlayer(Transform player, Transform refTr)
    {
        Vector3 visorDirection = getVisorDir(player, refTr);

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(visorDirection + refTr.position, Vector3.one * .032f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(refTr.position, visorDirection);
    }

    private void DrawGizmoTargets(List<Transform> targets, Transform refTr)
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < targets.Count - 1; i++)
        {
            Vector3 visorDirection = getVisorDir(targets[i], targets[i + 1]);

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(visorDirection + targets[i + 1].position, Vector3.one * .032f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(targets[i + 1].position, visorDirection);
        }
        Vector3 visorDirectionLoop = getVisorDir(targets[0], targets[targets.Count - 1]);

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(visorDirectionLoop + targets[targets.Count - 1].position, Vector3.one * .032f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(targets[targets.Count - 1].position, visorDirectionLoop);
    }

    public Vector3 getVisorDir(Transform target, Transform RefTr)
    {
        return target.transform.position - RefTr.position;
    }

    [PropertySpace(SpaceBefore = 5, SpaceAfter = 15)]
    [System.Serializable]
    public class moveData
    {
        [PropertySpace(SpaceBefore = 5, SpaceAfter = 15)]
        public Sc_movementPattern movementPattern;

        public List<Transform> targets;
        [Button(ButtonSizes.Small)]
        private void ResetTargets()
        {
            targets = new List<Transform>();
        }
        
        [HideInInspector]
        public int nextTarget;
        [HideInInspector]
        public Transform refTr;
        [HideInInspector]
        public AIPath path;
        [HideInInspector]
        public AIDestinationSetter destinationSetter;
        [HideInInspector]
        public Camera cam;
        [HideInInspector]
        public SpriteRenderer eye;
        [HideInInspector]
        public Vector3 screenPoint;
        [HideInInspector]
        public bool onScreen;
        [HideInInspector]
        public bool inDistance;
        [HideInInspector]
        public bool canPickNextDestination = true;

        [HideInInspector]
        public bool isChasing;
        [HideInInspector]
        public float iniSpeed;
        [HideInInspector]
        public Vector3 iniPos;

        public void resetValues(GameObject refGo)
        {
            nextTarget = 0;
            refTr = refGo.transform;
            path = refGo.GetComponent<AIPath>();
            destinationSetter = refGo.GetComponent<AIDestinationSetter>();
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            eye = refTr.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            isChasing = false;
            iniSpeed = path.maxSpeed;
            iniPos = refTr.position;
        }
    }

    public void SetupMovement(moveData data)
    {
        switch (TargetState)
        {
            case TargetType.ChasePlayer:
                //data.path.canMove = true;
                data.destinationSetter.target = data.targets[0];
                break;
            case TargetType.Targets:
                data.path.canMove = true;
                data.destinationSetter.target = data.targets[0];
                break;
            case TargetType.Stationary:
                data.path.canMove = false;
                data.destinationSetter.target = data.targets[0];
                break;
            case TargetType.Other:
                break;
        }
    }

    public moveData Move(moveData data)
    {
        switch (PlayerInSightState)
        {
            case PlayerInSightType.Chase:
                data = MovePlayerInSightChase(data);
                break;
            case PlayerInSightType.Stop:
                data = MovePlayerInSightStop(data);
                break;
            case PlayerInSightType.Ignore:
                break;
        }

        switch (TargetState)
        {
            case TargetType.ChasePlayer:
                data = MoveChasePlayer(data);
                break;
            case TargetType.Targets:
                data = MoveTargets(data);
                break;
            case TargetType.Stationary:
                if (data.path.canMove == true)
                {
                    data = MoveChasePlayer(data);
                }
                else
                {
                    data.eye.transform.DOMove(getVisorDir(data.targets[0], data.refTr).normalized * 0.1f + data.refTr.position, 0.12f);
                }
                break;
            case TargetType.Other:
                break;
        }

        return data;
    }

    public moveData MovePlayerInSightChase(moveData data)
    {
        data.screenPoint = data.cam.WorldToViewportPoint(data.refTr.position);
        data.onScreen = data.screenPoint.z > 0 && data.screenPoint.x > 0 && data.screenPoint.x < Screen.width * inCamLimit && data.screenPoint.y > 0 && data.screenPoint.y < Screen.height * inCamLimit;
        data.inDistance = (data.targets[0].transform.position - data.refTr.position).magnitude < playerDistanceLimit;
        if (data.onScreen && data.inDistance)
        {
            data.path.canMove = true;
            data.isChasing = true;
        }
        else
        {
            //path.canMove = false;
        }

        return data;
    }
    public moveData MovePlayerInSightStop(moveData data)
    {
        data.screenPoint = data.cam.WorldToViewportPoint(data.refTr.position);
        data.onScreen = data.screenPoint.z > 0 && data.screenPoint.x > 0 && data.screenPoint.x < Screen.width * inCamLimit && data.screenPoint.y > 0 && data.screenPoint.y < Screen.height * inCamLimit;
        data.inDistance = (data.targets[0].transform.position - data.refTr.position).magnitude < playerDistanceLimit;
        if (data.onScreen && data.inDistance)
        {
            data.path.canMove = false;
        }
        else
        {
            //path.canMove = true;
        }

        return data;
    }

    public moveData MoveChasePlayer(moveData data)
    {
        Vector3 visorDirection = getVisorDir(data.targets[0], data.refTr);
        data.eye.transform.DOMove(visorDirection * 0.04f + data.refTr.position, 0.12f);

        if (!data.onScreen || !data.inDistance)
        {
            data.path.maxSpeed = data.iniSpeed * 1.25f;
        }
        else
        {
            data.path.maxSpeed = data.iniSpeed;
        }
        

        return data;
    }
    public moveData MoveTargets(moveData data)
    {
        Vector3 visorDirection = getVisorDir(data.targets[data.nextTarget], data.refTr);
        data.eye.transform.DOMove(visorDirection * 0.04f + data.refTr.position, 0.12f);

        if (!data.onScreen || !data.inDistance)
        {
            data.path.maxSpeed = data.iniSpeed * 1.25f;
        }
        else
        {
            data.path.maxSpeed = data.iniSpeed;
        }

        if (data.path.reachedEndOfPath && data.canPickNextDestination)
        {
            data.canPickNextDestination = false;

            if(data.nextTarget == data.targets.Count - 1)
            {
                data.nextTarget = 0;
            }
            else
            {
                data.nextTarget += 1;
            }

            data.destinationSetter.target = data.targets[data.nextTarget];
        }
        if (!data.path.reachedEndOfPath && !data.canPickNextDestination)
        {
            data.canPickNextDestination = true;
        }


        return data;
    }
}