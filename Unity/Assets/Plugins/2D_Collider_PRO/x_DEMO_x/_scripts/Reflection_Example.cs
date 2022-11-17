using UnityEngine;
using System.Collections;

// Example class about how to use Reflection system

public class Reflection_Example : MonoBehaviour 
{

	public Animation door_anim;
	public Animation button_anim;






	public void OnRaycast()
	{
		button_anim.Play ("button_on");
		door_anim.Play ("door_open");
	}

	public void OnRaycastStop()
	{
		button_anim.Play ("button_off");
		door_anim.Play ("door_close");
	}




}
