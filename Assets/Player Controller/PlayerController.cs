using UnityEngine;
// ReSharper disable All
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private float speed;
	[SerializeField] private float jumpHeight;
	[SerializeField] private AnimationCurve speedRamp;
	private float speedRampTimerx;
	private float speedRampTimery;
	[SerializeField] private float internalVecChangeSpeed;
	[SerializeField] private float rampUpSpeed;
	[SerializeField] private float rampDownSpeed;
	private Vector2 interpolatedAxis = Vector2.zero;
	private Vector2 axis;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update()
	{
		Vector2 realAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (realAxis != Vector2.zero)
		{
			axis = realAxis;
		}
		interpolatedAxis = Vector2.MoveTowards(interpolatedAxis, axis, internalVecChangeSpeed);
		
		if (Mathf.Abs(realAxis.x) > 0) speedRampTimerx += Time.deltaTime * rampUpSpeed;
		else if (speedRampTimerx > 0) speedRampTimerx -= Time.deltaTime * rampDownSpeed;
		else if (speedRampTimerx < 0) speedRampTimerx = 0;
		
		if (Mathf.Abs(realAxis.y) > 0) speedRampTimery += Time.deltaTime * rampUpSpeed;
		else if (speedRampTimery > 0) speedRampTimery -= Time.deltaTime * rampDownSpeed;
		else if (speedRampTimery < 0) speedRampTimery = 0;
		speedRampTimerx = Mathf.Clamp(speedRampTimerx, 0, 1);
		speedRampTimery = Mathf.Clamp(speedRampTimery, 0, 1);
		rb.linearVelocity = new Vector3(interpolatedAxis.x * speedRamp.Evaluate(speedRampTimerx), 0,
		                                interpolatedAxis.y * speedRamp.Evaluate(speedRampTimery)) * speed;
	}
}