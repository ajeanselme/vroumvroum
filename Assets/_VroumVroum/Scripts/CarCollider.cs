using UnityEngine;

public class CarCollider : MonoBehaviour
{
    private CarController car;

    public AudioClip[] collision;
    public AudioSource playAudio;
    private int randomSon;

    private void Start()
    {
        car = transform.parent.GetComponent<CollisionController>().car;
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PropController>())
        {
            other.gameObject.GetComponent<PropController>().callCollision(car.theRB.velocity);
            car.reduceSpeed(other.gameObject.GetComponent<PropController>().weight);
            if (other.gameObject.CompareTag("Book"))
            {
                randomSon = Random.Range(0, 3);
                playAudio.clip = (AudioClip)collision[randomSon];
                playAudio.Play();
            }
            if (other.gameObject.CompareTag("Metal"))
            {
                randomSon = Random.Range(10, 12);
                playAudio.clip = (AudioClip)collision[randomSon];
                playAudio.Play();
            }
            if (other.gameObject.CompareTag("Glass"))
            {
                randomSon = Random.Range(8, 9);
                playAudio.clip = (AudioClip)collision[randomSon];
                playAudio.Play();
            }
            if (other.gameObject.CompareTag("Other"))
            {
                randomSon = Random.Range(4, 7);
                playAudio.clip = (AudioClip)collision[randomSon];
                playAudio.Play();
            }
            if (other.gameObject.CompareTag("Wood"))
            {
                randomSon = Random.Range(13, 15);
                playAudio.clip = (AudioClip)collision[randomSon];
                playAudio.Play();
            }
        }
        else
        {
            if (!other.transform.IsChildOf(car.transform) && other.gameObject != car.theRB.gameObject)
            {
                transform.GetComponentInParent<CollisionController>().callCollision(other);
            }
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (!other.transform.IsChildOf(car.transform) && other.gameObject != car.theRB.gameObject)
    //     {
    //         transform.GetComponentInParent<CollisionController>().callCollision(this, other);
    //     }
    // }
}
