using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectThrow : MonoBehaviour
{
    public GameObject objectThrowing;
    public float throwStrength;
    public Camera mainCamera;
    public float scaleObject = 1f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator DestroyTrhownObject(GameObject _gameObject)
    {
        yield return new WaitForSeconds(3f);
        Object.Destroy(_gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Convertir la position de la souris en position de monde
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 1; // distance de la caméra
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Créer l'objet à la position de la souris
            GameObject newObject = Instantiate(objectThrowing, worldPosition, Quaternion.identity);

            // Lancer de l'objet
            newObject.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * throwStrength, ForceMode.Impulse);
            newObject.GetComponent<Transform>().localScale.Set(scaleObject, scaleObject, scaleObject);
            StartCoroutine(DestroyTrhownObject(newObject));
        }
    }
}
