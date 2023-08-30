using UnityEngine;
using System.Collections;
using PowerTools;

namespace PowerTools.Anim
{

/** Attachments Component: 
		Example component that attaches child game objects to corresponding Anim Node
		The index in the list of attachments corresponds to the Anim node it'll be attached to
*/
[RequireComponent(typeof(SpriteAnimNodes))] 
public class Attachments : MonoBehaviour 
{
	
	[Tooltip("Add the name of child game object to attach to each SpriteAnim Node")]
	public Transform[] m_attachments = null;

	// A reference to the SpriteAnimNodes component on this object
	SpriteAnimNodes m_nodes = null;

	// We use LateUpdate to ensure the animation has updated before updating attachment positions
	void LateUpdate()
	{
		// Here we cache the SpriteAnimNodes component for efficiency
		if ( m_nodes == null )
			m_nodes = GetComponent<SpriteAnimNodes>();		

		// Loop through the list of attachment names, and attach to the corresponding node's index
		for ( int i = 0; i < m_attachments.Length; ++i )
		{
			m_nodes.SetTransformFromNode( m_attachments[i], i );
		}		
	}

}

}
