using UnityEngine;
using System.Collections;
using PowerTools;

namespace PowerTools.Anim
{

/** PlayRandomAnim Component: 
		Simple component that chooses a random animation from a list
		Also has a function to change animations by id
*/
public class PlayRandomAnim : MonoBehaviour 
{
	// A list of clips to select from
	public AnimationClip[] m_anims = null;

	void Start()
	{
		// On start, choose a random animation
		SetClip(Random.Range(0,m_anims.Length));
	}


	// Play the animation from the list of clips, using it's index
	public void SetClip(int index) 
	{
		// Check there's an anim component and a corresponding id for the clip, and play it!
		SpriteAnim animComponent = GetComponentInChildren<SpriteAnim>(true);
		if ( animComponent != null && index >= 0 && index < m_anims.Length )
		{
			animComponent.Play( m_anims[index] );
		}

	}

}

}