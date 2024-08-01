using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
	[SerializeField]
	private Material lineMaterial;

    private List<LineCollection> lineCollections = new List<LineCollection>();

    public class LineCollection
    {
        private Color color;

        private List<Vector3> points;

        private bool visible;

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        public List<Vector3> Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
            }
        }
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        public LineCollection(Color color)
        {
            this.color = color;
            points = new List<Vector3>();
            visible = true;
        }

        public void AddPoint(Vector3 position)
        {
            points.Add(position);
        }

        public void Clear()
        {
            points.Clear();
        }
    }

    void OnPostRender()
	{
		if (!GlobalData.playMode || GlobalData.debugMode)
		{
            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            foreach (var lineCollection in lineCollections)
			{
                if (lineCollection.Visible)
                {
                    GL.Color(lineCollection.Color);
                    for (int i = 0; i < lineCollection.Points.Count; i++)
                        GL.Vertex(lineCollection.Points[i]);
                }
			}
            GL.End();
        }
	}

    public LineCollection AddCollection(LineCollection collection)
    {
        lineCollections.Add(collection);

        return collection;
    }
    public void RemoveCollection(LineCollection collection)
    {
        lineCollections.Remove(collection);
    }
}