using UnityEngine;
using System.Collections.Generic;

public class TheStack : MonoBehaviour {

	private List<GameObject> theStack;

	private const float ORIGINAL_TILE_SIZE = 4.0f;
	private const float ERROR_MARGIN = 0.2f;
	private const float STACK_GAIN_SIZE = 0.25f;
	private const int COMBO_START_GAIN = 5;

	public List<Color32> gameColors;
	public Material stackMat;
	
	private Vector2 stackSize = new Vector3(ORIGINAL_TILE_SIZE, ORIGINAL_TILE_SIZE);
	private Vector3 xPos1;
	private Vector3 xPos2;
	private Vector3 zPos1;
	private Vector3 zPos2;
	private Vector3 lastTilePosition;
	private Vector3 secondaryPosition;
	private float tileSpeed = 0.65f;
	private float tileTransition = 0.0f;
	private float colorLerp = 0.0f;

	private int scoreCount = 0;
	private int combo = 0;

	private bool isMovingOnX;
	private bool gameOver = false;

	// Use this for initialization
	void Start () {
		isMovingOnX = true;

		secondaryPosition = new Vector3(0f, scoreCount + 0.5f, 0f);

		theStack = new List<GameObject>();
		for (int i = 0; i < transform.childCount-1; i++) {
			GameObject tile = transform.GetChild (i).gameObject;
			theStack.Add (tile);
			ColorMesh(tile.GetComponent<MeshFilter>().mesh);
		}
		SpawnTile();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameOver){
			theStack[theStack.Count - 1].AddComponent<Rigidbody>();
			return;
		}
		if(Input.GetMouseButtonDown(0)){
			if(PlaceTile()){
				colorLerp = Mathf.PingPong(scoreCount * 0.1f, 9.0f);
				scoreCount++;
				SpawnTile();
			} else
			{
				EndGame();
			}
		}

		MoveTile();
	}

    private void SpawnTile(){
		lastTilePosition = theStack[theStack.Count - 1].transform.localPosition;

		GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Transform tileTransform = tile.transform;

		tileTransform.parent = this.transform;
		tileTransform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
		tileTransform.position = new Vector3(lastTilePosition.x, scoreCount + 0.5f, lastTilePosition.z);

		tile.GetComponent<MeshRenderer>().material = stackMat;
		ColorMesh(tile.GetComponent<MeshFilter>().mesh);
		
		theStack.Add(tile);
	}

	private void ColorMesh(Mesh mesh){
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];

		 for (int i = 0; i < vertices.Length; i++){
			colors[i] = Lerp10(gameColors[0], gameColors[1], gameColors[2], gameColors[3], gameColors[4], gameColors[5], gameColors[6]
			, gameColors[7], gameColors[8], gameColors[9]);
		}
		mesh.colors32 = colors;
	}

    private Color32 Lerp10(Color32 a, Color32 b, Color32 c, Color32 d, Color32 e
	, Color32 f, Color32 g, Color32 h, Color32 i, Color32 j)
    {
		if(colorLerp >= 0.0f && colorLerp <= 1.0f){
			return Color32.Lerp(a, b, colorLerp);
		} else if (colorLerp > 1.0f && colorLerp <= 2.0f){
			return Color32.Lerp(b, c, colorLerp - 1);
		} else if (colorLerp > 2.0f && colorLerp <= 3.0f){
			return Color32.Lerp(c, d, colorLerp - 2);
		} else if (colorLerp > 3.0f && colorLerp <= 4.0f){
			return Color32.Lerp(d, e, colorLerp - 3);
		} else if (colorLerp > 4.0f && colorLerp <= 5.0f){
			return Color32.Lerp(e, f, colorLerp - 4);
		} else if (colorLerp > 5.0f && colorLerp <= 6.0f){
			return Color32.Lerp(f, g, colorLerp - 5);
		} else if (colorLerp > 6.0f && colorLerp <= 7.0f){
			return Color32.Lerp(g, h, colorLerp - 6);
		} else if (colorLerp > 7.0f && colorLerp <= 8.0f){
			return Color32.Lerp(h, i, colorLerp - 7);
		} else if (colorLerp > 8.0f && colorLerp <= 9.0f){
			return Color32.Lerp(i, j, colorLerp - 8);
		} else
		{
			return Color.grey;
		}
    }

    private void CreateRubble (Vector3 pos, Vector3 scale){
		GameObject rubble = GameObject.CreatePrimitive(PrimitiveType.Cube);
		rubble.transform.localPosition = pos;
		rubble.transform.localScale = scale;
		rubble.AddComponent<Rigidbody>();

		rubble.GetComponent<MeshRenderer>().material = stackMat;
		ColorMesh(rubble.GetComponent<MeshFilter>().mesh);
	}

    private void MoveTile()
    {
		xPos1 = new Vector3(5.0f, scoreCount + 0.5f, secondaryPosition.z);
		xPos2 = new Vector3(-5.0f, scoreCount + 0.5f, secondaryPosition.z);
		zPos1 = new Vector3(secondaryPosition.x, scoreCount + 0.5f, 5.0f);
		zPos2 = new Vector3(secondaryPosition.x, scoreCount + 0.5f, -5.0f);

    	GameObject tile = theStack[theStack.Count - 1];
		if(isMovingOnX){
			tile.transform.position = Vector3.Lerp (xPos1, xPos2, Mathf.PingPong(Time.time * tileSpeed, 1.0f));
		} else
		{
			tile.transform.position = Vector3.Lerp (zPos1, zPos2, Mathf.PingPong(Time.time * tileSpeed, 1.0f));
		}
    }

	private bool HasReachedBounds(GameObject tile){
		Vector3 tilePosition = tile.transform.localPosition;
		if(isMovingOnX){
			if(tilePosition.x > stackSize.x || tilePosition.x < -stackSize.x){
				return true;
			}
		} else {
			if(tilePosition.z > stackSize.y || tilePosition.z < -stackSize.y){
				return true;
			}
		}

		return false;
	}

    private bool PlaceTile()
    {
		if(isMovingOnX){
			return PlaceTileOnX();
		} else
		{
			return PlaceTileOnZ();
		}
    }

    private bool PlaceTileOnX()
    {
		GameObject tile = theStack[theStack.Count - 1];

        float deltaX = lastTilePosition.x - tile.transform.position.x;

		// Cut the tile on X-Axis
		if(Mathf.Abs(deltaX) > ERROR_MARGIN){
			combo = 0;
			stackSize.x -= Mathf.Abs(deltaX);
			if(stackSize.x <= 0){
				return false;
			}

			float middle = lastTilePosition.x + tile.transform.localPosition.x / 2;
			tile.transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
			CreateRubble(
				new Vector3
					(((tile.transform.position.x - lastTilePosition.x) > 0)
					? tile.transform.position.x + (tile.transform.localScale.x / 2)
					: tile.transform.position.x - (tile.transform.localScale.x / 2)
					, tile.transform.position.y
					, tile.transform.position.z),
				new Vector3(Mathf.Abs(deltaX), 1, tile.transform.localScale.z)
				);
			tile.transform.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount + 0.5f, lastTilePosition.z);

		} else
		{
			if(combo >= COMBO_START_GAIN){
				stackSize.x += STACK_GAIN_SIZE;
				if(stackSize.x >= ORIGINAL_TILE_SIZE){
					stackSize.x = ORIGINAL_TILE_SIZE;
				}
				float middle = lastTilePosition.x + tile.transform.localPosition.x / 2;
				tile.transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
				tile.transform.position = new Vector3(middle - (lastTilePosition.x / 2), scoreCount + 0.5f, lastTilePosition.z);
			}
			combo ++;
			tile.transform.position = new Vector3(lastTilePosition.x, scoreCount + 0.5f, lastTilePosition.z);
		}
		secondaryPosition.x = tile.transform.localPosition.x;
		isMovingOnX = !isMovingOnX;
		return true;
    }

    private bool PlaceTileOnZ()
    {
    	GameObject tile = theStack[theStack.Count - 1];

        float deltaZ = lastTilePosition.z - tile.transform.position.z;

		// Cut the tile on Z-Axis
		if(Mathf.Abs(deltaZ) > ERROR_MARGIN){
			combo = 0;
			stackSize.y -= Mathf.Abs(deltaZ);
			if(stackSize.y <= 0){
				return false;
			}

			float middle = lastTilePosition.z + tile.transform.localPosition.z / 2;
			tile.transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
			CreateRubble(
				new Vector3(
					tile.transform.position.x
					, tile.transform.position.y
					, ((tile.transform.position.z - lastTilePosition.z) > 0)
					? tile.transform.position.z + (tile.transform.localScale.z / 2)
					: tile.transform.position.z - (tile.transform.localScale.z / 2)
				),
				new Vector3(tile.transform.localScale.x, 1, Mathf.Abs(deltaZ))
			);
			tile.transform.position = new Vector3(lastTilePosition.x, scoreCount + 0.5f, middle - (lastTilePosition.z / 2));

		} else
		{
			if(combo >= COMBO_START_GAIN){
				stackSize.y += STACK_GAIN_SIZE;
				if(stackSize.y >= ORIGINAL_TILE_SIZE){
					stackSize.y = ORIGINAL_TILE_SIZE;
				}
				float middle = lastTilePosition.z + tile.transform.localPosition.z / 2;
				tile.transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
				tile.transform.position = new Vector3(lastTilePosition.x, scoreCount + 0.5f, middle - (lastTilePosition.z / 2));
			}
			combo ++;
			tile.transform.position = new Vector3(lastTilePosition.x, scoreCount + 0.5f, lastTilePosition.z);
		}
		secondaryPosition.z = tile.transform.localPosition.z;
		isMovingOnX = !isMovingOnX;
		return true;
    }

	public int getScoreCount(){
		return scoreCount;
	}

	public void EndGame(){
		gameOver = true;
	}

}