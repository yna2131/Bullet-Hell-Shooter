using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 인스펙터 창에서 설정할 수 있도록 public 변수 추가
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public int health = 3;

    // Input Axis 값을 받을 전역변수 선언
    float hAxis;
    float vAxis;

    bool wDown;
    bool iDown;
    bool sDown1;
    bool sDown2;

    bool isSwap;
    bool isFireReady;

    Vector3 moveVec;

    Rigidbody rigid;
    Animator anim;

    // 무기 교체를 위한 변수 선언
    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;

    void Awake()
    {
        // Rigidbody 변수를 GetComponent() 함수로 초기화
        rigid = GetComponent<Rigidbody>();
        // Animator 변수를 GetComponentInChildren() 함수로 초기화
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Attack();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        // GetAxisRaw() : Axis 값을 정수로 반환하는 함수
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");

    }

    void FreezeRotation()
    {
        // AngularVelocity : 물리 회전 속도
        rigid.angularVelocity = Vector3.zero;
    }

    void Move()
    {
        // Vector3.normalized : 벡터의 크기를 1로 만드는 함수
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // bool 형태 조건 ? true 일 때 값 : false 일 때 값 (삼항 연산자)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        // Animator.SetBool() : Animator의 bool 값을 설정하는 함수
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        // 현재 플레이어의 월드좌표(tranasform.position)을 뷰포트 기준 좌표로 변환
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        // Mathf.Clamp01(값) : 입력된 값이 0 ~ 1 사이를 벗어나지 못하게 강제로 조정
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);

        // 뷰포트 기준 좌표를 월드좌표로 변환
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewPos);
        transform.position = worldPos;
    }

    void Turn()
    {
        // Lookat(): 특정 방향을 바라보게 하는 함수
        transform.LookAt(transform.position + moveVec);
    }

    void Attack()
    {
        // 무기가 없거나 교체 중일 때 공격 불가
        if (equipWeapon == null || isSwap)
            return;

        // 공격딜레이에 시간을 더해주고, 공격가능 여부를 확인
        // 공격 딜레이가 0일 때 공격 가능
        fireDelay += Time.deltaTime;
        // 공격 딜레이가 무기의 공격속도보다 높을 때 공격 가능
        bool isFireReady = equipWeapon.rate < fireDelay;

        // 무기를 들고 있을 때, 바로 자동으로 버튼을 누르지 않고도 계속해서 공격하도록 설정
        if (equipWeapon.type == Weapon.Type.Range)
        {
            equipWeapon.Use();
            anim.SetTrigger("doShot");
            fireDelay = 15f;
        }

    }

    // 무기 교체 함수
    void Swap()
    {
        // 무기 중복 교체, 없는 무기 확인을 위한 조건
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;

        if (sDown1 || sDown2)
        {
            // 기존에 장착된 무기가 있을 경우 비활성화
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        // 상호작용 함수가 작동될 수 있는 조건 작성
        if (iDown && nearObject != null)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    // 트리거 이벤트인 OnTriggerStay, Exit 사용
    void OnTriggerStay(Collider other)
    {
        // Weapon 태그를 조건으로 하여 로직 작성
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }

        Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }

    void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage();

            Destroy(collision.gameObject);
        }
    }
    
    public void TakeDamage()
    {
        // 플레이어의 체력을 1 감소
        health--;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
