using UnityEngine;
using System.Collections;
using PowerTools;

namespace PowerTools.Anim
{

/** 
	ExampleEffect: An example of a simple fire-and-forget animated effect
		- Can either destroy itself after a time, or after an animation plays
		- Has some simple gravity/speed variables
		- Can spawn other effects when created/destroyed
*/
public class ExampleEffect : MonoBehaviour {

	[Tooltip("If true, the effect destroys itself after its animation ends")]
	public bool m_destroyOnAnimEnd = true;

	[Tooltip("If the timer is > 0, the effect will destroy itself after that many seconds")]
	public float m_destroyAfterTime = -1;

	[Tooltip("A list of objects to spawn at the same time as this one")]
	public GameObject[] m_spawnOnCreate = null;

	[Tooltip("A list of objects to spawn when this object destroys itself")]
	public GameObject[] m_spawnOnDestroy = null;

	[Tooltip("Effect initial speed in rotation direction")]
	public float m_speed = 0;

	[Tooltip("Effect gravity (accelleration downwards)")]
	public float m_gravity = 0;

	[Tooltip("Whether the effect should rotate to the direction it's pointing")]
	public bool m_faceDirection = false;

	// Timer used to destroy self after a set time
	float m_timer = 0;

	// Anim component used to check if an animation has finished playing
	SpriteAnim m_anim = null;

	Vector3 m_velocity = Vector3.zero;

	void Start()
	{
		m_velocity = transform.rotation * Vector2.right * m_speed;

		foreach ( GameObject toSpawn in m_spawnOnCreate )
			Instantiate(toSpawn, transform.position,transform.rotation);
	}

	// Update is called once per frame
	void Update() 
	{	
		// Create a flag to set if we should destroy this effect
		bool shouldDestroy = false;

		if ( m_destroyAfterTime > 0 )
		{
			// Update timer and destroy after time's up
			m_timer += Time.deltaTime;
			if ( m_timer >= m_destroyAfterTime )
				shouldDestroy = true;
		}

		if ( m_destroyOnAnimEnd )
		{
			// Cache anim component
			if ( m_anim == null )
				m_anim = GetComponent<SpriteAnim>();

			// Check if animation has finished playing, and destroy if it has
			if ( m_anim != null && m_anim.IsPlaying() == false )
				shouldDestroy = true;
		}

		// Apply gravity
		m_velocity += Vector3.down * m_gravity * Time.deltaTime;
		// Apply velocity
		transform.position += m_velocity * Time.deltaTime;

		// Face direction
		if ( m_faceDirection )
			transform.rotation = Quaternion.FromToRotation(Vector2.right, m_velocity.normalized);

		// Destroy if the flag is set
		if ( shouldDestroy )
		{
			foreach ( GameObject toSpawn in m_spawnOnDestroy )
				Instantiate(toSpawn, transform.position,transform.rotation);

			Destroy(gameObject);	
		}
	}
}

}
