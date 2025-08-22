using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    // Start is called before the first frame update
    
        public float returnDelay = 3f;
        public GameObject impactEffect;
    

        private bool hasHit = false;
             public int damage;
        private float timer = 0f;

    
    public void Initialize(float delay)
        {
            returnDelay = delay;
        }

        void Update()
        {
            // 如果没有击中任何物体，计时后销毁
            if (!hasHit)
            {
                timer += Time.deltaTime;
                if (timer >= returnDelay)
                {
                    Destroy(gameObject);
                }
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            // 碰撞到物体时
            if (!hasHit)
            {
                hasHit = true;

                // 生成碰撞特效
                if (impactEffect != null)
                {
                ContactPoint2D contact = collision.contacts[0];
                Instantiate(impactEffect, contact.point, Quaternion.identity);
            }

                // 移除物理组件
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Destroy(rb);
                }

                // 短暂延迟后销毁
                StartCoroutine(DestroyAfterDelay(0.5f));
            }
        }

        IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

    }
