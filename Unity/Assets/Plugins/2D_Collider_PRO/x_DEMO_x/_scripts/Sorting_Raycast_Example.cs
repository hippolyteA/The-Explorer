using UnityEngine;
using UnityEngine.UI;
using System.Collections;


// Example class about how to use Sorting based Raycast system

public class Sorting_Raycast_Example : MonoBehaviour 
{


	public Button random_btn;
	public SpriteRenderer[] sprite_rends;
	public TextMesh[] text_meshes;

	int[,] random = new int[,]{{0,1,2},{0,2,1},{1,0,2},{1,2,0},{2,1,0},{2,0,1}};
	Collider2D selected_collider;





	// Use this for initialization
	void Start () 
	{
		random_btn.onClick.AddListener(() => Randomize());
	}




	void Randomize()
	{
		int r = Random.Range (0,6);

		sprite_rends[0].sortingOrder = random[r,0];
		sprite_rends[1].sortingOrder = random[r,1];
		sprite_rends[2].sortingOrder = random[r,2];

		text_meshes [0].text = "Sorting order - " + sprite_rends [0].sortingOrder.ToString ();
		text_meshes [1].text = "Sorting order - " + sprite_rends [1].sortingOrder.ToString ();
		text_meshes [2].text = "Sorting order - " + sprite_rends [2].sortingOrder.ToString ();
	}




	// Update is called once per frame
	void Update ()
	{
	

		if(Input.GetMouseButtonDown(0))
		{
			Vector3 mpos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			RaycastHit2D rhit = _2D_Collider_Pro.Sorting_Raycast(new Vector2(mpos.x , mpos.y) , Vector3.forward);

			if(rhit.collider != null)
			{
				selected_collider = rhit.collider;
				selected_collider.transform.localScale += Vector3.one * 2;
			}

		}




		if(Input.GetMouseButtonUp(0))
		{
			if(selected_collider != null)
				selected_collider.transform.localScale -= Vector3.one * 2;

			selected_collider = null;
		}




	}



}
