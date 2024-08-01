using UnityEngine;

public class OutOfBoundsForegroundGenerator : MonoBehaviour
{
	private LevelContainer levelContainer;

	private Mesh mesh;

	private bool cachedPlayMode = false;

	void Start()
	{
		levelContainer = GetComponentInParent<LevelContainer>();

		mesh = GetComponent<MeshFilter>().mesh;

		UpdateMesh();
	}

	void LateUpdate()
	{
		//if (levelContainer.PhoneScreen.transform.hasChanged || levelContainer.Background.transform.hasChanged || cachedPlayMode != GlobalData.playMode)
		//{
		//	UpdateMesh();
		//	levelContainer.PhoneScreen.transform.hasChanged = false;
		//	levelContainer.Background.transform.hasChanged = false;
		//	cachedPlayMode = GlobalData.playMode;
		//}
	}

	private void UpdateMesh()
	{
		//const float zDpeth = 0.0f;

		//var screenBounds = levelContainer.PhoneScreen.bounds;
		//var backgroundBounds = levelContainer.Background.bounds;

		//mesh.Clear();

		//// 5            6
		////               
		////     1    2    
		////               
		////     0    3    
		////              
		//// 4            7
		//Vector3[] vertices =
		//{
		//	new Vector3(backgroundBounds.min.x, backgroundBounds.min.y, zDpeth),
		//	new Vector3(backgroundBounds.min.x, backgroundBounds.max.y, zDpeth),
		//	new Vector3(backgroundBounds.max.x, backgroundBounds.max.y, zDpeth),
		//	new Vector3(backgroundBounds.max.x, backgroundBounds.min.y, zDpeth),
		//	new Vector3(screenBounds.min.x, screenBounds.min.y, zDpeth),
		//	new Vector3(screenBounds.min.x, screenBounds.max.y, zDpeth),
		//	new Vector3(screenBounds.max.x, screenBounds.max.y, zDpeth),
		//	new Vector3(screenBounds.max.x, screenBounds.min.y, zDpeth)
		//};
		//for (int i = 0; i < vertices.Length; i++)
		//{
		//	vertices[i] = new Vector3(Mathf.Clamp(vertices[i].x, screenBounds.min.x, screenBounds.max.x), Mathf.Clamp(vertices[i].y, screenBounds.min.y, screenBounds.max.y), vertices[i].z);
		//	vertices[i] = transform.worldToLocalMatrix.MultiplyPoint(vertices[i]);
		//	vertices[i].z = zDpeth;
		//}

		////   \     2/3     /
		////    \           /
		////     -----------
		////     |         |
		//// 0/1 |         | 4/5
		////     |         |    
		////     -----------
		////    /           \
		////   /     6/7     \
		//int[] triangles = 
		//{
		//	4, 5, 1, 4, 1, 0,
		//	1, 5, 6, 1, 6, 2,
		//	3, 2, 6, 3, 6, 7,
		//	4, 0, 3, 4, 3, 7
		//};

		//Color[] colors = new Color[vertices.Length];
		//for (int i = 0; i < colors.Length; i++)
		//	colors[i] = Color.black;

		//mesh.vertices = vertices;
		//mesh.triangles = triangles;
		//mesh.colors = colors;
	}
}