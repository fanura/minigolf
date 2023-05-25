using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

//Raycast System menggunakan IPointerDownHandler
public class BallController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float force;
    [SerializeField] Transform aimWorld;
    [SerializeField] LineRenderer aimLine;
    bool shoot;
    bool shootingMode;
    float forceFactor;
    Vector3 forceDirection;
    Ray ray;
    Plane plane;

    int shootCount;

    public bool ShootingMode { get => shootingMode;}
    public int ShootCount { get => shootCount;}

    public UnityEvent<int> onBallShooted = new UnityEvent<int>();

    private void Update()
    {
        //Raycast System Manual
        // if (Input.GetMouseButtonDown(0))
        // {
        //     var ray = Camera.main.ScreenPointToRay((Input.mousePosition));
        //     // var ray = Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f));
        //     if(Physics.Raycast(ray, out var hitInfo) && hitInfo.collider == col){
        //         shoot = true;
        //     }
        // }

        if (ShootingMode)
        {
            //Ketika di klik
            if (Input.GetMouseButtonDown(0)){
                aimLine.gameObject.SetActive(true);
                aimWorld.gameObject.SetActive(true);
                plane = new Plane(Vector3.up, this.transform.position);
            }
            //Ketika di tahan
            else if (Input.GetMouseButton(0))
            {

                //draw aim
                // aimLine.transform.position = ballScreenPos;
                // var positions = new Vector3[]{ballScreenPos,Input.mousePosition};
                // aimLine.SetPositions(positions);
                
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                plane.Raycast(ray,out var distance);
                forceDirection = (this.transform.position-ray.GetPoint(distance));
                forceDirection.Normalize();


                //force factor
                var mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var ballViewportPos = Camera.main.WorldToViewportPoint(this.transform.position);
                var pointerDirection = ballViewportPos - mouseViewportPos;
                pointerDirection.z = 0;
                pointerDirection.z *= Camera.main.aspect;
                pointerDirection.z = Mathf.Clamp(pointerDirection.z,-0.5f,0.5f);
                forceFactor = Vector2.Distance(ballViewportPos, mouseViewportPos)*2;
                
                //aim visuals
                aimWorld.transform.position = this.transform.position;
                aimWorld.forward=forceDirection;
                aimWorld.localScale = new Vector3(1,1,0.5f+forceFactor);

                var ballScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
                var mouseScreenPos = Input.mousePosition;
                ballScreenPos.z = 1;
                mouseScreenPos.z = 1;
                var positions = new Vector3[]{
                    Camera.main.ScreenToWorldPoint(ballScreenPos),
                    Camera.main.ScreenToWorldPoint(mouseScreenPos)
                };
                aimLine.SetPositions(positions);
                aimLine.endColor = Color.Lerp(Color.blue, Color.red, forceFactor);
           
            }
            //Ketika diangkat
            else if (Input.GetMouseButtonUp(0))
            {
                shoot=true;
                shootingMode = false;
                aimLine.gameObject.SetActive(false);
                aimWorld.gameObject.SetActive(false);
            }
        }
    }
    private void FixedUpdate()
    {
        if (shoot)
        {
            shoot = false;
            AddForce(forceDirection * force * forceFactor, ForceMode.Impulse);
            shootCount+=1;
            onBallShooted.Invoke(shootCount);
        }
        if (rb.velocity.sqrMagnitude < 0.01f && rb.velocity.sqrMagnitude != 0)
        {
            rb.velocity = Vector3.zero;
        }
    }
    public bool IsMove()
    {
        return rb.velocity != Vector3.zero;
    }

    public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse){
        rb.useGravity = true;
        rb.AddForce(force, forceMode);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(this.IsMove()){
            return;
        }
        shootingMode = true;
    }
}