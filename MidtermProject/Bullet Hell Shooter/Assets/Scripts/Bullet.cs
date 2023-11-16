using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
  public int damage;
  private float angle;
  public void SetAngle(float newAngle)
  {
    angle = newAngle;
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    float speed = 10.0f;

    // 각도를 라디안으로 변환
    float angleInRadians = angle * Mathf.Deg2Rad;

    // 삼각함수를 이용해, X축과 Z축의 이동량을 계산
    Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, angle, 0));
    Vector3 direction = rotationMatrix.MultiplyVector(Vector3.forward);

    // 위에서 계산한 이동량을 speed에 곱해서 이동시킴
    transform.Translate(direction * speed * Time.deltaTime);

    // 총알이 메인 카메라의 사야 밖으로 나가면 삭제
    OnOutOfSight();

  }
  
  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.tag == "Floor")
    {
      Destroy(gameObject);
    }
  }

  // 화면 밖으로 나가면 삭제
  void OnOutOfSight()
  {
    if (Camera.main == null)
    {
      return;
    }

    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

    if (screenPos.x < 0 || screenPos.x > Camera.main.pixelWidth || screenPos.y < 0 || screenPos.y > Camera.main.pixelHeight)
    {
      Boss bossScript = GameObject.Find("Boss").GetComponent<Boss>();
      bossScript.setCurrentBullets(bossScript.getCurrentBullets() - 1);

      Destroy(gameObject);
    }
  }
}