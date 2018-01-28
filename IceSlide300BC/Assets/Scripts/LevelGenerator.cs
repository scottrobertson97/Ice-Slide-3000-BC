using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
	public GameObject wallPrefab;
	public GameObject acornPrefab;
	public Texture2D map;

	// Use this for initialization
	void Start () {
		//GenerateLevel ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Level GenerateLevel(){
		Level level = new Level();
		level.board = new Board[map.width, map.height];
		level.acorns = new List<GameObject> ();

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
			Instantiate (wallPrefab, new Vector3 (x, y, 0), Quaternion.identity);
			return Board.Wall;
		} else if (pixelColor == Color.blue) {
			GameObject.Find ("Player").transform.position = new Vector3 (x, y, 0);
			return Board.Floor;
		} else if (pixelColor == Color.green) {
			GameObject acorn = (GameObject)Instantiate (acornPrefab, new Vector3 (x, y, 0), Quaternion.identity);
			level.acorns.Add (acorn);
			return Board.Acorn;
		}else {
			return Board.Floor;
		}
	}
}
