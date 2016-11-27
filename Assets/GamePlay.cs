using UnityEngine;
using System.Collections;

public class GamePlay : MonoBehaviour {

	private int[,] area; 
	private ArrayList snake;
	private ArrayList directions;

	private enum Direction {
		NORTH, SOUTH, WEST, EAST
	}
		
	private Direction d = Direction.WEST;

	private float speed = 0.3f; // 1 square per second
	private float st; // start time

	private bool running = false;
	private bool gameover = false;
	// private bool pressed = false;

	private int grid = 30;

	private int createCube(Vector3 p) {
		if (p.x < 0 || p.x >= grid || p.z < 0 || p.z >= grid) {
			return -1;
		}
		if (area [(int)p.x, (int)p.z] >= 1) {
			return -1;
		}
		int v = 0;
		GameObject temp = GameObject.CreatePrimitive (PrimitiveType.Cube);
		temp.name = "S" + "." + p.x + "." + p.z;
		temp.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
		temp.transform.position = p;
		temp.GetComponent<Renderer> ().material.color = new Color (0.0f, 0.4f, 0.0f);
		// snake.Add (temp);
		snake.Insert (0, temp);
		if (area [(int)p.x, (int)p.z] == -1) {
			v = 1;
			GameObject.Destroy (GameObject.Find ("F." + p.x + "." + p.z));
		}
		area [(int)p.x, (int)p.z] = 1; // 1 is for the snake
		return v;
	}

	private void createFood() {
		bool lookingfor = true;
		while (lookingfor) {
			int px = Random.Range (0, grid-1);
			int pz = Random.Range (0, grid-1);

			if (area [px, pz] == 0) {
				lookingfor = false;
				area [px, pz] = -1;
				GameObject temp = GameObject.CreatePrimitive (PrimitiveType.Cube);
				temp.name = "F" + "." + px + "." + pz;
				temp.transform.localScale = new Vector3 (0.7f, 0.7f, 0.7f);
				temp.transform.position = new Vector3(px, 0.5f, pz);
				temp.GetComponent<Renderer> ().material.color = new Color (0.4f, 0.0f, 0.0f);
				temp.AddComponent<Rotator> ();
			}
		}
	}

	private void createWall() {
		/* this is a temporary function. */
		for (int i = 4; i < 12; i++) {
			area [i, 3] = 3;
			GameObject temp = GameObject.CreatePrimitive (PrimitiveType.Cube);
			temp.name = "W" + "." + i + "." + 3;
			temp.transform.position = new Vector3 (i, 0.5f, 3);
			temp.GetComponent<Renderer> ().material.color = new Color (0.2f, 0.2f, 0.2f);
		}
	}

	// Use this for initialization
	void Start () {
		snake = new ArrayList();
		directions = new ArrayList (); 
		area = new int[grid, grid];
		for (int i = 0; i < grid; i++) {
			for (int j = 0; j < grid; j++) {
				area [i, j] = 0;
			}
		}
		GameObject temp = GameObject.CreatePrimitive (PrimitiveType.Cube);
		temp.transform.localScale = new Vector3 (grid, 0.1f, grid);
		float tt = ( (float)(grid - 1) )/ 2.0f;
		temp.transform.position = new Vector3 (tt, 0.0f, tt);
		//temp.name = "A" + "." + i + "." + j;
		temp.GetComponent<Renderer> ().material.color = new Color (0.5f, 0.5f, 0.5f);

		for (int i = 0; i < 3; i++) {
			createCube (new Vector3 ((grid/2) + 2 - i, 0.5f, grid/2));
		}
		createWall ();
		createFood ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameover)
			return;

		if (Input.GetKey (KeyCode.Space) && !running) {
			st = Time.time;
			running = true;
		} 

		if (!running)
			return;

		Direction temp; 
		if (directions.Count > 0) {
			temp = (Direction)directions [directions.Count - 1];
		} else
			temp = d; 

		if (Input.GetKeyUp (KeyCode.UpArrow) && temp != Direction.SOUTH) {
			directions.Add(Direction.NORTH);
			// d = Direction.NORTH;
		} else if (Input.GetKeyUp (KeyCode.DownArrow) && temp != Direction.NORTH) {
			directions.Add(Direction.SOUTH);
			// d = Direction.SOUTH;
		} else if (Input.GetKeyUp (KeyCode.LeftArrow) && temp != Direction.EAST) {
			directions.Add(Direction.WEST);
			// d = Direction.WEST;
		} else if (Input.GetKeyUp (KeyCode.RightArrow) && temp != Direction.WEST) {
			directions.Add(Direction.EAST);
			// d = Direction.EAST;
		}

		/*

		if (Input.GetAxis("Horizontal") < 0 && d != Direction.EAST && !pressed) {
			d = Direction.WEST;
			pressed = true;
		} else if(Input.GetAxis("Horizontal") > 0 && d != Direction.WEST && !pressed) {
			d = Direction.EAST; 
			pressed = true;
		}

		if (Input.GetAxis ("Vertical") < 0 && d != Direction.NORTH && !pressed) {
			d = Direction.SOUTH; 
			pressed = true;
		} else if (Input.GetAxis ("Vertical") > 0 && d != Direction.SOUTH && !pressed) {
			d = Direction.NORTH; 
			pressed = true;
		}*/

		if (Time.time - st > speed) {
			st = Time.time; 
			Vector3 p = ((GameObject)snake [0]).transform.position;

			if (directions.Count > 0) {
				d = (Direction)directions [0];
				directions.RemoveAt (0);
			}

			switch (d) {
			case Direction.EAST:
				p.x += 1;
				break;
			case Direction.NORTH:
				p.z += 1;
				break;
			case Direction.SOUTH:
				p.z -= 1;
				break;
			case Direction.WEST:
				p.x -= 1;
				break;
			}
			int s = createCube (p);
			if (s == 0) {
				GameObject t = (GameObject)snake [snake.Count - 1];
				snake.RemoveAt (snake.Count - 1);
				area [(int)t.transform.position.x, (int)t.transform.position.z] = 0;
				GameObject.Destroy (t);
			} else if (s == 1) {
				createFood ();
				//speed = 5.0f / (float)(snake.Count+2);
			} else if (s == -1) {
				gameover = true;
			}
			// pressed = false;
		}

	}
}
