using UnityEngine;

public class ViewCone : PersonProperty
{
    [SerializeField]
    private float radius = 5.0f;

    [SerializeField]
    private float arc = 50.0f;

    [SerializeField]
    private float turnRate = 5.0f;

    [SerializeField]
    private float drainRate = 1.5f;

    private Vector3 targetVector;
    private Vector3 currentVector;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] indices;

    private const int segments = 20;

    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            radius = value;
        }
    }

    public float Arc
    {
        get
        {
            return arc;
        }
        set
        {
            arc = value;
        }
    }

    public float TurnRate
    {
        get
        {
            return turnRate;
        }
        set
        {
            turnRate = value;
        }
    }

    public float DrainRate
    {
        get
        {
            return drainRate;
        }
        set
        {
            drainRate = value;
        }
    }

    public override Type PropertyType
    {
        get
        {
            return Type.ViewCone;
        }
    }

    protected override void Start()
    {
        base.Start();

        targetVector = person.DirectionVector;
        currentVector = targetVector;

        GetComponentInChildren<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        mesh = GetComponentInChildren<MeshFilter>().mesh;
        mesh.Clear();

        vertices = new Vector3[segments + 1];
        indices = new int[(segments - 1) * 3];
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        targetVector = person.DirectionVector;

        float targetAngle = MathUtility.GetAngleFromVector(targetVector);
        float currentAngle = MathUtility.GetAngleFromVector(currentVector);

        if (Vector3.Angle(targetVector, currentVector) < turnRate * 2.0f)
        {
            currentVector = targetVector;
        }
        else
        {
            float difference1 = Mathf.Abs(targetAngle - currentAngle);
            float difference2 = Mathf.Abs(targetAngle - 360.0f - currentAngle);
            float difference3 = Mathf.Abs(targetAngle + 360.0f - currentAngle);

            float turnDelta = 0.0f;

            if (currentAngle < targetAngle)
                turnDelta += turnRate;
            else if (currentAngle > targetAngle)
                turnDelta -= turnRate;

            if (difference2 < difference1 || difference3 < difference1)
                turnDelta *= -1.0f;

            currentAngle += turnDelta * GlobalData.timeMultiplier;

            currentVector = MathUtility.GetVectorFromAngle(currentAngle);

            if (Vector3.Angle(targetVector, currentVector) < turnRate * 2.0f)
                currentVector = targetVector;
        }

        UpdateMesh();
    }

    protected override void EditorUpdate()
    {
        base.EditorUpdate();

        currentVector = targetVector;
    }

    protected override void GameUpdate()
    {
        base.GameUpdate();

        if (person != GlobalData.player)
        {
            if (CanSee(GlobalData.player))
                GlobalData.player.AddPhobia(new PhobiaPersonYouKnowSeesYou(GlobalData.player, person, drainRate));
        }
        else
        {
            foreach (var npc in GlobalData.allNPCs)
            {
                if (CanSee(npc) && npc.GetProperty(Type.DontLookAt))
                    GlobalData.player.AddPhobia(new PhobiaThingYouDontWantToSee(GlobalData.player, person, drainRate));
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        int currentVertex = 0;
        int currentIndex = 0;

        vertices[currentVertex++] = Vector3.zero;

        float angle = MathUtility.GetAngleFromVector(currentVector);

        float increment = arc / segments;

        float currentAngle = angle - arc * 0.5f;
        for (int i = 0; i < segments; i++)
        {
            currentAngle += increment;

            //vertices[currentVertex++] = MathUtility.GetVectorFromAngle(currentAngle) * radius;

            var target = MathUtility.GetVectorFromAngle(currentAngle) * radius;

            var ray = new Ray(person.transform.position, target);
            RaycastHit[] hits = Physics.RaycastAll(ray, radius);

            float minDistance = 1000000.0f;
            Vector3 minHit = Vector3.zero;

            bool foundHit = false;
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    SolidObject solidObject = hit.transform.GetComponentInChildren<SolidObject>();

                    if (solidObject != null && solidObject.canSeeOver)
                        continue;

                    var distance = Vector3.Distance(person.transform.position, hit.point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minHit = hit.point;
                        foundHit = true;
                    }
                }

                if (foundHit)
                    vertices[currentVertex++] = minHit - person.transform.position;
            }
            
            if (!foundHit)
            {
                vertices[currentVertex++] = target;
            }
        }

        for (int i = 1; i < segments; i++)
        {
            indices[currentIndex++] = 0;
            indices[currentIndex++] = i - 1;
            indices[currentIndex++] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = indices;
    }

    public bool CanSee(Person otherPerson)
    {
        if (Vector3.Distance(person.transform.position, otherPerson.transform.position) < radius)
        {
            if (Vector3.Angle(otherPerson.transform.position - person.transform.position, currentVector) < arc * 0.5f)
            {
                var ray = new Ray(person.transform.position, (otherPerson.transform.position - person.transform.position).normalized);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                DWPObject closestHit = null;
                Vector3 closestHitPoint = Vector3.zero;
                foreach (var hit in hits)
                {
                    DWPObject obj = null;

                    if (hit.collider.name == "RayHit" || hit.collider.name.Contains("Player"))
                    {
                        var otherPersonHit = hit.collider.GetComponentInParent<Person>();

                        if (otherPersonHit != null && otherPersonHit != person)
                            obj = otherPersonHit;
                    }
                    else
                    {
                        SolidObject solidObject = hit.transform.GetComponentInChildren<SolidObject>();

                        if (solidObject != null && !solidObject.canSeeOver)
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

                if (closestHit == otherPerson)
                {
                    var viewportPoint = Camera.main.WorldToViewportPoint(otherPerson.transform.position);

                    //Don't count people offscreen
                    if (viewportPoint.x > 0.0f && viewportPoint.x < 1.0f && viewportPoint.y > 0.0f && viewportPoint.y < 1.0f)
                        return true;
                }
            }
        }

        return false;
    }
}