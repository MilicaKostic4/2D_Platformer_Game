using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; //Serijalizovano da bi moglo da se edituje u Unity-ju
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded; //Pracenje da li je igrac na zemlji

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y); //Kretanje po horizontali

        //Okretanje igraca prilikom kretanja po horizontali
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Input.GetKey(KeyCode.Space) && grounded)
            Jump();

        //Podesavanje animacija
        anim.SetBool("run", horizontalInput != 0); //ne pokrece se run sve dok se ne pritisne leva ili desna strelica
        anim.SetBool("grounded", grounded);
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, speed);
        anim.SetTrigger("jump");
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            grounded = true;
    }//registrovanje kada je objekat dotakao tlo

}
