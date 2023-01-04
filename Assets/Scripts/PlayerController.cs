
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float speed = 75.0f;
	public float rotateSpeed = 5f;
	public float floatSpeed = 35.0f;
	public float maxWalkSpeed = 5f;
	public float maxFloatSpeed = 8f;
	public float step = 0.6f;
	public float deadzone = 0.01f;
	public float jumpForce = 750f;
	public float jumpDelay = 0.5f;
	public float jumpGapRequirement = 0.2f;
	public float floorGap = 0.2f;
	public float artificalGravity = 9.80665f;

	private float jumpedTime = 0;
	private Rigidbody2D rigid;
	private CapsuleCollider2D capsule;
	private Vector2 targetDir;
	private Vector2 targetPoint;
	// Start is called before the first frame update
	void Start() {
		rigid = GetComponent<Rigidbody2D>();
		capsule = GetComponent<CapsuleCollider2D>();
		jumpedTime = Time.time;
		targetDir = Vector2.up;
		targetPoint = transform.position - transform.up;
	}

	// Update is called once per frame
	void Update() {
		var down = -transform.up;
		var capHeight = (capsule.size.y * 0.5f) + 0.1f;
		var bottom = transform.position + (down * capHeight);
		var top = transform.position + (transform.up * capHeight);
		var hit = Physics2D.Raycast(bottom, down, jumpGapRequirement);
		var xaxis = Input.GetAxis("Horizontal");
		var yaxis = Input.GetAxis("Vertical");
		var localVelocity = transform.TransformVector(rigid.velocity);
		var currentSpeed = localVelocity.magnitude;
		var moveDirection = Vector2.zero;
		var speed = this.speed;
		var jump = false;
		if (hit && hit.rigidbody != rigid) {
			Debug.DrawLine(bottom, hit.point, Color.red);
			if (Mathf.Abs(xaxis) > deadzone) {
				var direction = Vector2.right * xaxis;
				direction = transform.TransformVector(direction);
				targetPoint = hit.point;
				var angle = Vector2.Angle(localVelocity, direction);
				speed = this.speed;
				if ((angle <= 175 && angle >= 185) || currentSpeed <= maxWalkSpeed) {
					Vector3 trypoint = targetPoint + (direction.normalized * step);
					var forwardDir = (trypoint - transform.position).normalized;
					var start = transform.position+(forwardDir);

					Debug.DrawRay(start, forwardDir, Color.green);
					var downHit = Physics2D.Raycast(start, forwardDir, step);
					if (downHit) {
						moveDirection = (downHit.point - rigid.position).normalized;
						targetPoint = downHit.point;
						targetDir = downHit.normal;
					} else {
						moveDirection = direction;
					}
				}
			}
			if (yaxis > deadzone && jumpedTime + jumpDelay <= Time.time) {
				jump = true;
				jumpedTime = Time.time;
			}
		} else {
			targetPoint = Vector2.zero;
			Debug.DrawRay(bottom, down * jumpGapRequirement, Color.green);
			if (Mathf.Abs(xaxis) > deadzone) {
				var direction = Vector2.right * xaxis;
				direction = transform.TransformVector(direction);
				var angle = Vector2.Angle(localVelocity, direction);
				speed = this.floatSpeed;
				if ((angle <= 175 && angle >= 185) || currentSpeed <= maxFloatSpeed) {
					moveDirection = direction;
					targetPoint = transform.TransformPoint( Vector2.right * xaxis);
				}
			}
		}
		if (targetPoint != Vector2.zero) {
			Debug.DrawRay(transform.position, moveDirection.normalized, Color.cyan);
			Debug.DrawRay(targetPoint, targetDir, Color.blue);
			Vector2 up = transform.up;
			Vector2 target = Vector2.Lerp(rigid.position, targetPoint + (up * floorGap), speed * Time.deltaTime);
			rigid.AddForce(target - rigid.position, ForceMode2D.Impulse);
		}
		var dir = Vector2.MoveTowards(transform.up, targetDir, rotateSpeed * Time.deltaTime);

		rigid.MoveRotation(Vector2.SignedAngle(Vector3.up, dir));
		if(jump) {
			rigid.AddForce(transform.up * jumpForce);
		}
		rigid.AddForce(transform.up * -artificalGravity);
	}
}
