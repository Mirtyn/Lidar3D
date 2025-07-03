using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    [SerializeField] private Material heldMaterial;
    [SerializeField] private Material normalMaterial;

    private LayerMask mapLayer = 7;
    private LayerMask pusheableObjectLayer = 8;
    private LayerMask buttonLayer = 9;
    private LayerMask buttonBaseLayer = 11;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] boxCollideClips;

    public Vector3 SpawnPoint = new Vector3();
    private float maxDepth = -10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == mapLayer || collision.gameObject.layer == pusheableObjectLayer || collision.gameObject.layer == buttonLayer || collision.gameObject.layer == buttonBaseLayer)
        {
            var rndNum = Random.Range(0, boxCollideClips.Length);

            audioSource.clip = boxCollideClips[rndNum];
            audioSource.Play();
        }
    }

    private void OnEnable()
    {
        SpawnPoint = transform.position;
    }

    private void Update()
    {
        if (this.transform.position.y < maxDepth)
        {
            this.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            this.transform.position = SpawnPoint;
        }
    }

    public void ObjectHeld()
    {
        //this.gameObject.GetComponent<Renderer>().material = heldMaterial;
    }

    public void ObjectLetGoOf()
    {
        //this.gameObject.GetComponent<Renderer>().material = normalMaterial;
    }
}
