using UnityEngine;

public class MoveAcrossScreen : MonoBehaviour
{
    public float speed = 3f;
    public float left_bound = -5f;
    public float right_bound = 5f; 
    private bool move_right = true;

    void Update()
    {
        if (move_right)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            if (transform.position.x >= right_bound)
            {
                move_right = false;
                Flip();
            }
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x <= left_bound)
            {
                move_right = true;
                Flip();
            }
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
