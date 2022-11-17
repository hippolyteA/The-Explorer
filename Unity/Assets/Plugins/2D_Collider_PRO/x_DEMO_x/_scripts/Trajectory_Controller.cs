using UnityEngine;
using System.Collections;


// Example class about how to use Trajectory system

public class Trajectory_Controller : MonoBehaviour 
{

	public _2D_Line_Trajectory trajectory;
	public _2D_Throw_Object ball;


	bool update = true;


	// Use this for initialization
	void Start () 
	{
		// initialize trajectory 
		trajectory.Set_Throw_Object (ball);
		trajectory.Show ();

        StartCoroutine(test());
    }


	
	void Update()
	{

		if (update) 
		{
            //ball lookat mouse
            Vector3 mouse_pos = Vector3.up;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouse_pos.z = 0;
			ball.transform.up = mouse_pos - ball.transform.position;

			//update trajectory path
			trajectory.Update_Values ();
		}


		//if (Input.GetKeyDown (KeyCode.Space)) 
		//{
		//	ball.Throw ();
		//	trajectory.Hide();
		//	update = false;
		//}


		//if (Input.GetKeyDown (KeyCode.R)) 
		//{
		//	Restart_Scene();
		//}

	}

    //lazy test, I know
    IEnumerator test()
    {
        while(1 == 1)
        {
            yield return new WaitForSeconds(5);
            ball.Throw();
            trajectory.Hide();
            update = false;
            print("a");
        }
    }


	void Restart_Scene()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

}
