using System.Collections;
using UnityEngine;

namespace Player.Animations
{
    public class IKHands : MonoBehaviour
    {
        private Animator animator;
        public Transform leftHandObj;
        public Transform attachLeft;
        public bool canBeUsed;
		public bool isUsed;
        [Range(0, 1)] public float leftHandPositionWeight;
        [Range(0, 1)] public float leftHandRotationWeight;
        private Transform blendToTransform;
		private Coroutine co;
		private float _currentLeftHandPositionWeight;
		private float _currentLeftHandRotationWeight;
		
        private void Awake()
        {
	        _currentLeftHandPositionWeight = leftHandPositionWeight;
	        _currentLeftHandRotationWeight = leftHandRotationWeight;
            animator = GetComponent<Animator>();
        }

		/// <summary>
		/// If there is movement and/or rotation data in the animation for the Left Hand, use IK to 
		/// set the position of the Left Hand of the character.
		/// </summary>
		private void OnAnimatorIK(int layerIndex)
        {
            if (leftHandObj) 
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
				if (attachLeft) 
				{
					animator.SetIKPosition(AvatarIKGoal.LeftHand, attachLeft.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand, attachLeft.rotation);
				}
            }
        }

		/// <summary>
		/// Pauses IK while Warrior uses Left Hand during an animation.
		/// </summary>
		public void SetIKPause(float pauseTime)
		{
			StopAllCoroutines();
			co = StartCoroutine(_SetIKPause(pauseTime));
		}

		private IEnumerator _SetIKPause(float pauseTime)
		{
			float t = 0f;
			while (t < 1) {
				t += Time.deltaTime / 0.1f;
				leftHandPositionWeight = Mathf.Lerp(1, 0, t);
				leftHandRotationWeight = Mathf.Lerp(1, 0, t);
				yield return null;
			}
			yield return new WaitForSeconds(pauseTime - 0.2f);
			t = 0f;
			while (t < 1) {
				t += Time.deltaTime / 0.1f;
				leftHandPositionWeight = Mathf.Lerp(0, 1, t);
				leftHandRotationWeight = Mathf.Lerp(0, 1, t);
				yield return null;
			}
		}

		public void SetIKOff()
		{
			if (canBeUsed)
			{
				StopAllCoroutines();
				leftHandPositionWeight = 0;
				leftHandRotationWeight = 0;
			}
		}

		public void SetIKOn()
		{
			if (canBeUsed)
			{
				StopAllCoroutines();
				leftHandPositionWeight = _currentLeftHandPositionWeight;
				leftHandRotationWeight = _currentLeftHandRotationWeight;
			}
		}
    }
}