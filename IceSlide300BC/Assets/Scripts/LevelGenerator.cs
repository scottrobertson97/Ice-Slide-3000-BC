using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
	public GameObject wallPrefab;
	public GameObject acornPrefab;
	public GameObject floorSprite;
	private Texture2D map;

	// Use this for initialization
	void Start () {
		//GenerateLevel ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Level GenerateLevel(Texture2D _map){
		this.map = _map;
		Level level = new Level();
		level.board = new Board[map.width, map.height];
		level.acorns = new List<Acorn> ();
        level.objects = new List<GameObject>();
        level.playerPosition = new Vector3();

		floorSprite.GetComponent<SpriteRenderer> ().size = new Vector2 (map.width, map.height);

		for (int x = 0; x < map.width; x++) {
			for (int y = 0; y < map.height; y++) {
				level.board[x,y] = GenerateTile (x, y, ref level);
			}
		}

		return level;
	}

	public Board GenerateTile(int x, int y, ref Level level){
		Color pixelColor = map.GetPixel (x, y);

		if (pixelColor == Color.black) {
			GameObject wall = Instantiate (wallPrefab, new Vector3 (x, y, 0), Quaternion.identity);
            level.objects.Add(wall);
			return Board.Wall;
		} else if (pixelColor == Color.blue) {
			GameObject.Find ("Player").transform.position = new Vector3 (x, y, 0);
            level.playerPosition = new Vector3(x, y, 0);
			return Board.Floor;
		} else if ((255 * pixelColor.g) == 255 && pixelColor.b != 1) {
			GameObject a = Instantiate (acornPrefab, new Vector3 (x, y, 0), Quaternion.identity);
            Acorn acorn = a.GetComponent<Acorn>();
            acorn.setHits((int)(255 * pixelColor.b));
            level.objects.Add(a);
			level.acorns.Add (acorn);
			return Board.Acorn;
		}else {
			return Board.Floor;
		}
	}
}
