using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement params")]
    [SerializeField] private float speed; //Serijalizovano da bi moglo da se edituje u Unity-ju
    [SerializeField] private float jumpPower;

    [Header("Coyote time")]
    [SerializeField] private float coyoteTime; //Koliko dugo igrac moze biti u vazduhu pre nego sto skoci
    private float coyoteCounter; //Koliko je vremena proslo

    [Header("Multiple jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter; //Koliko skokova imam u ovom momentu

    [Header("Wall jumping")]
    [SerializeField] private float wallJumpX; //Koliko mogu da se krecem po horizontali
    [SerializeField] private float wallJumpY; //Koliko mogu visoko da skocim

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown; //delay izmedju skokova
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Okretanje igraca prilikom kretanja po horizontali
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //Podesavanje animacija
        anim.SetBool("run", horizontalInput != 0); //ne pokrece se run sve dok se ne pritisne leva ili desna strelica
        anim.SetBool("grounded", isGrounded());

        /*
        //skakanje uz zid
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y); //Kretanje po horizontali

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
                //kada igrac skoci uz zid kaci se za njega i ne moze da padne
            }
            else
                body.gravityScale = 7; //brzina padanja

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();

                if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
                    SoundManager.instance.PlaySound(jumpSound);
            }
                
        }
        else
            wallJumpCooldown += Time.deltaTime;
        */

        //Skok
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        //Podesiva visina skoka
        if(Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (onWall())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime; //Resetovanje brojaca kada je igrac na zemlji
                jumpCounter = extraJumps; //Resetovanje brojaca skokova
            }
            else
                coyoteCounter -= Time.deltaTime; //Odbrojavanje
        }
            
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;
        SoundManager.instance.PlaySound(jumpSound);

        /*
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            
        }
        else if(onWall() && !isGrounded())
        {
            //kada je igrac zakacen za zid, ako je pritisnuta strelica u pravcu zida - igrac nastavlja da se penje po njemu, u suprotnom pada
            if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z); // okretanje igraca kada se otkaci sa zida
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            
            wallJumpCooldown = 0;
        }
        */

        if (onWall())
            WallJump();
        else
        {
            if (isGrounded())
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            else
            {
                if(coyoteCounter > 0)
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                else
                {
                    if(jumpCounter > 0) //Skakanje i smanjivanje brojaca skokova
                    {
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }
            coyoteCounter = 0; //Resetovanje da bi se izbegli dupli skokovi
        }
    }

    private void WallJump()
    {
        //Uzimam trenutni velocity i dodajem jos snage na to
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
    }

    private bool isGrounded()
    {
        // kreira se virtuelna linija koja kada dodje u dodir sa objektom koji ima collider vraca se true
        // ako je igrac u vazduhu collider ce biti no, pa je isGrounded false
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
 

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    } // moze da napadne ako se ne krece i ako je na zemlji tj. nije zakacen za zid

}
