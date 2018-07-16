using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolProperties : EnemyProperties {

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public Vector3 initialPosition;
    public float wanderRadius;
   
    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    public float maxSpeed = 7;

    public bool grounded;
    public Vector2 groundNormal;
    public Rigidbody2D rb2d;
    public Vector2 velocity;
    public ContactFilter2D contactFilter;
    public RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    public List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    public const float minMoveDistance = 0.001f;
    public const float shellRadius = 0.01f;

}