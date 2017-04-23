using UnityEngine;
using System.Collections;

public class AnimationHelper
{
	public static readonly Vector3 forward = new Vector3(1,1,0);
	public static readonly Vector3 right = new Vector3(1,-1,0);

	public static void UpdateAnimator(Animator animator, float speed, Vector3 dir)
	{
		animator.SetFloat ("Speed", speed);
		
		if (dir.magnitude != 0) {
			if (Vector3.Dot (dir, right) > 0) {
				animator.transform.localScale = new Vector3(
					Mathf.Abs(animator.transform.localScale.x),
					animator.transform.localScale.y,
					animator.transform.localScale.z
				);
			} else {
				animator.transform.localScale = new Vector3(
					- Mathf.Abs(animator.transform.localScale.x),
					animator.transform.localScale.y,
					animator.transform.localScale.z
				);
			}
		}
	}
}

