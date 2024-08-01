using System.Collections.Generic;
using UnityEngine;

public class Approacher : PersonProperty
{
    private bool playingAction = false;

    private NPCActionController oldActionController;

    private float startTime = 0.0f;
    private float npcTurnAourndStart = 0.0f;
    private ObjectAction.MovementDirection foundDirection = ObjectAction.MovementDirection.None;
    private Vector3 otherPersonFoundPosition = Vector3.zero;
    private Person.MovementType otherPersonFoundMovementType = Person.MovementType.none;
    private bool npcUnPaused = false;

    private float adjustedTime = 0.0f;

    private NPC lastNPC;

    public override Type PropertyType
    {
        get
        {
            return Type.Approacher;
        }
    }
    
    protected override void GameUpdate()
    {
        base.GameUpdate();

        adjustedTime += Time.fixedDeltaTime * GlobalData.timeMultiplier;

        if (playingAction)
        {
            var oppositeMovementType = Person.MovementType.none;

            switch (foundDirection)
            {
                case ObjectAction.MovementDirection.Left:
                    oppositeMovementType = Person.MovementType.idleRight;
                    break;
                case ObjectAction.MovementDirection.Right:
                    oppositeMovementType = Person.MovementType.idleLeft;
                    break;
                case ObjectAction.MovementDirection.Up:
                    oppositeMovementType = Person.MovementType.idleDown;
                    break;
                case ObjectAction.MovementDirection.Down:
                    oppositeMovementType = Person.MovementType.idleUp;
                    break;
                default:
                    break;
            }

            if (lastNPC != null && adjustedTime - startTime > 0.5f && lastNPC.ActionController.Paused)
                lastNPC.SetMovementType(oppositeMovementType);

            if (lastNPC == null)
            {
                switch (otherPersonFoundMovementType)
                {
                    case Person.MovementType.idleLeft:
                    case Person.MovementType.walkLeft:
                        GlobalData.player.SetMovementType(Person.MovementType.idleLeft);
                        break;
                    case Person.MovementType.idleRight:
                    case Person.MovementType.walkRight:
                        GlobalData.player.SetMovementType(Person.MovementType.idleRight);
                        break;
                    case Person.MovementType.idleUp:
                    case Person.MovementType.walkUp:
                        GlobalData.player.SetMovementType(Person.MovementType.idleUp);
                        break;
                    case Person.MovementType.idleDown:
                    case Person.MovementType.walkDown:
                        GlobalData.player.SetMovementType(Person.MovementType.idleDown);
                        break;
                    default:
                        break;
                }

                GlobalData.player.transform.position = otherPersonFoundPosition;
            }

            var actionController = (person as NPC).ActionController;

            var lastAction = actionController.LastAction;

            if (lastAction != null)
            {
                var lastActionIndex = actionController.Actions.IndexOf(lastAction);

                if (lastActionIndex == 1)
                {
                    npcTurnAourndStart = adjustedTime;

                    if (lastNPC == null)
                        GlobalData.player.AddPhobia(new PhobiaPersonWantsToTalkToYou(GlobalData.player, person));
                }
                else if (lastActionIndex == 2)
                {
                    if (!npcUnPaused && lastNPC != null && adjustedTime - npcTurnAourndStart > 0.5f)
                    {
                        lastNPC.ActionController.Paused = false;
                        npcUnPaused = true;
                    }
                }
                else if (lastActionIndex == 3)
                {
                    if (!npcUnPaused && lastNPC != null)
                    {
                        lastNPC.ActionController.Paused = false;
                        npcUnPaused = true;
                    }

                    playingAction = false;
                    actionController.Actions.Clear();
                    (person as NPC).ActionController = oldActionController;
                    oldActionController.Paused = false;
                }
            }
        }
        else
        {
            var ray = new Ray(person.transform.position, person.DirectionVector);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            DWPObject closestHit = null;
            Vector3 closestHitPoint = Vector3.zero;
            foreach (var hit in hits)
            {
                DWPObject obj = null;

                if (hit.collider.name == "RayHit")
                {
                    var otherPerson = hit.collider.GetComponentInParent<Person>();

                    if (otherPerson != null && otherPerson != person)
                        obj = otherPerson;
                }
                else
                {
                    SolidObject solidObject = hit.transform.GetComponentInChildren<SolidObject>();

                    if (solidObject != null)
                        obj = solidObject;
                }

                if (obj != null)
                {
                    if (closestHit == null || Vector3.Distance(ray.origin, obj.transform.position) < Vector3.Distance(ray.origin, closestHitPoint))
                    {
                        closestHit = obj;
                        closestHitPoint = obj.transform.position;
                    }
                }
            }

            if (CouldHitPlayer())
            {
                if (closestHit == null || Vector3.Distance(ray.origin, GlobalData.player.transform.position) < Vector3.Distance(ray.origin, closestHitPoint))
                {
                    closestHit = GlobalData.player;
                    closestHitPoint = GlobalData.player.transform.position;
                }
            }

            if (closestHit != null && closestHit is Person && closestHit.GetComponentInChildren<Approacher>() == null && closestHit != lastNPC)
            {
                var otherPerson = closestHit as Person;

                var projection = otherPerson.transform.position - (person.transform.position + Vector3.Project(otherPerson.transform.position - person.transform.position, person.DirectionVector));

                // Wait until the person is at or slightly past the approacher
                if (projection.magnitude < 0.001f || Vector3.Dot(otherPerson.DirectionVector, projection) > 0.0f)
                    SetUpActions(otherPerson);
            }
        }
    }

    private void SetUpActions(Person otherPerson)
    {
        foundDirection = person.Direction;

        oldActionController = (person as NPC).ActionController;
        oldActionController.Paused = true;

        lastNPC = null;
        if (otherPerson.ItemType == ItemInfo.Type.NPC)
            lastNPC = otherPerson as NPC;

        if (lastNPC != null)
        {
            lastNPC.ActionController.Paused = true;
            npcUnPaused = false;
        }

        List<ObjectAction> actions = new List<ObjectAction>();

        var distance = Mathf.Round(Vector3.Distance(person.transform.position, otherPerson.transform.position)) - 1.0f;

        {
            var action = new ObjectAction();
            action.direction = foundDirection;
            action.distance = distance;

            actions.Add(action);
        }
        {
            var action = new ObjectAction();
            action.wait = true;
            action.waitTime = 2.0f;
            action.direction = foundDirection;

            actions.Add(action);
        }
        {
            var action = new ObjectAction();
            action.distance = distance;
            action.direction = ObjectAction.OppositeDirection(foundDirection);

            actions.Add(action);
        }
        {
            var action = new ObjectAction();
            action.wait = true;
            action.waitTime = 2.0f;
            action.direction = foundDirection;

            actions.Add(action);
        }

        (person as NPC).ActionController = new NPCActionController(person as NPC, NPC.speed, actions, person.CurrentMovementType);
        
        otherPersonFoundPosition = otherPerson.transform.position;
        otherPersonFoundMovementType = otherPerson.CurrentMovementType;

        playingAction = true;
        startTime = adjustedTime;
    }

    private bool CouldHitPlayer()
    {
        if (Vector3.Dot(GlobalData.player.transform.position - person.transform.position, person.DirectionVector) > 0.0f)
        {
            var projection = GlobalData.player.transform.position - (person.transform.position + Vector3.Project(GlobalData.player.transform.position - person.transform.position, person.DirectionVector));

            return projection.magnitude < 0.5f;
        }

        return false;
    }
}