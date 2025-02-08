using UnityEngine;
using System.Collections;
using PowerTools;

namespace PowerTools.Anim
{


/** 
	ExampleCharacter: An example of a character that uses features of PowerSprite Animator components
		- Character has Idle, Walk and Shoot animations, set in it's inspector
		- Character plays animations based on whether player is pressing right/left or shoot. And goes back to idle when finished.
		- Character uses anim tags to spawn effects (look in AnimWalk and AnimShoot animations to see them)
		- The character also has a gun attached to an AnimNode (See the Attachment component)
*/
public class ExampleCharacter : MonoBehaviour 
{

	// The speed of the character, set in the inspector
	public float m_speed = 0;

	// Idle and run animations, drag your animations into these fields in the inspector
	public AnimationClip m_idle = null;
	public AnimationClip m_run = null;
	public AnimationClip m_shoot = null;

	public GameObject m_prefabShootEffect = null;

	SpriteAnim m_anim = null;


	void Update () 
	{
		// Store the sprite anim component if we haven't already
		if ( m_anim == null )
			m_anim = GetComponent<SpriteAnim>();

		// If we're currently shooting, don't do anything else
		if ( m_anim.IsPlaying(m_shoot) )
			return;

		// Handle movement
		// Check if we're pressing left/right, and set a direction variable if we are
		float direction = 0;
		if ( Input.GetKey(KeyCode.LeftArrow) )
			direction = -1;

		if ( Input.GetKey(KeyCode.RightArrow) )
			direction = 1;
					
		if ( direction == 0 )
		{
			// If there's no direction pressed, return to the idle animation
			if ( m_anim.Clip != m_idle ) // (check we're not already in the animation first though)
				m_anim.Play(m_idle); 
		}
		else
		{
			// If direction is set, play the run animation (after checking we're not already playing it)
			if ( m_anim.Clip != m_run ) 
				m_anim.Play(m_run);

			// Move the character in the chosen direction
			transform.Translate(new Vector3(direction * m_speed * Time.deltaTime, 0, 0 ) );
			if ( transform.localScale.x != direction )
				transform.localScale = new Vector3( direction, 1, 1 );			
		}		

		// Shooting stuff!

		// Choose weapon with the '1' or '2' keys
		if ( Input.GetKeyDown(KeyCode.Alpha1) )
			GetComponentInChildren<PlayRandomAnim>().SetClip(0);
		if ( Input.GetKeyDown(KeyCode.Alpha2) )
			GetComponentInChildren<PlayRandomAnim>().SetClip(1);

		// Start shooting animation with the spacebar
		if ( Input.GetKeyDown(KeyCode.Space))
			m_anim.Play(m_shoot);

	}


	////////////////////////////////////////////////////////////////////////////
	// ANIMATION EVENTS!

	// This function is called from an animation event "Shoot" in the shoot animation. It's a simple example of an animation event. 
	void Shoot()
	{		
		// Spawn an effect on the position from node 1 in the animation
		Instantiate( m_prefabShootEffect, GetComponent<SpriteAnimNodes>().GetPosition(1), Quaternion.Euler(0,0, GetComponent<SpriteAnimNodes>().GetAngle(1)) );
	}

	/** This function is called from an animation event "RapidFire" in the shoot animation
		- The event has the "Anim prefix" ticked, so we add that prefix to the function in code. Doing this makes it clearer where the function's called from.
	*/
	void AnimRapidFire()
	{			
		// If spacebar is held whe this tag is hit, we rewind the animation a bit so we can fire again
		if ( Input.GetKey(KeyCode.Space) )
			m_anim.Time -= 0.25f;		
	}

	/** This function is called from an animation event "Spawn" in the run animation
		- The event has the "Anim prefix" ticked, so we add that to the function in code. Doing this makes it clearer where the function's called from.
		- The event has an object parameter, so you can specify which object is spawned.
	*/
	void AnimSpawn( Object prefabToSpawn )
	{
		// Spawn an effect on the position from node 1 in the animation
		Instantiate( prefabToSpawn, GetComponent<SpriteAnimNodes>().GetPosition(1), Quaternion.identity );
	}

}

}