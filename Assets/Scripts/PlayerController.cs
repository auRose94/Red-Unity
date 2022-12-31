using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float speed = 75.0f;
	public float floatSpeed = 35.0f;
	public float maxWalkSpeed = 5f;
	public float maxFloatSpeed = 8f;
	public float step = 0.6f;
	public float deadzone = 0.01f;
	public float jumpForce = 750f;
	public float jumpDelay = 0.5f;
	public float jumpGapRequirement = 0.2f;

	private float jumpedTime = 0;
	Rigidbody2D rigid;
	CapsuleCollider2D capsule;
	// Start is called before the first frame update
	void Start() {
		rigid = GetComponent<Rigidbody2D>();
		capsule = GetComponent<CapsuleCollider2D>();
		jumpedTime = Time.time;
	}

	// Update is called once per frame
	void Update() {
		var down = -transform.up;
		var capHeight = (capsule.size.y * 0.5f) + 0.05f;
		var bottom = transform.position + (down * capHeight);
		var hit = Physics2D.Raycast(bottom, down);
		var xaxis = Input.GetAxis("Horizontal");
		var yaxis = Input.GetAxis("Vertical");
		if (hit && hit.distance <= jumpGapRequirement) {
			Debug.DrawLine(bottom, hit.point, Color.red);
			if (yaxis > deadzone && jumpedTime + jumpDelay <= Time.time) {
				var up = transform.TransformDirection(Vector3.up);
				rigid.AddForce(up * jumpForce);
				jumpedTime = Time.time;
			}
			if (Mathf.Abs(xaxis) > deadzone && transform.TransformVector(rigid.velocity).magnitude <= maxWalkSpeed) {
				var direction = new Vector2(xaxis, 0).normalized;
				var impulse = transform.TransformDirection(direction) * step;
				rigid.AddRelativeForce(impulse * speed * Time.deltaTime, ForceMode2D.Impulse);
			}
		} else {
			Debug.DrawLine(bottom, bottom + down, Color.green);
			if (Mathf.Abs(xaxis) > deadzone && transform.TransformVector(rigid.velocity).magnitude <= maxFloatSpeed) {
				var direction = new Vector2(xaxis, 0).normalized;
				var impulse = transform.TransformDirection(direction) * step;
				rigid.AddRelativeForce(impulse * floatSpeed * Time.deltaTime, ForceMode2D.Impulse);
			}
		}



	}
}
