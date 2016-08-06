using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour
{

	public float letterPause = 0.4f;
	public AudioClip typeSound1;
	public AudioClip typeSound2;

	string message;
	Text textComp;

	bool typing;

	void Update ()
	{
		if (Input.anyKeyDown && !typing)
		{
			TypeText ();
		}
	}


	void Start ()
	{
		textComp = GetComponent<Text> ();
		message = textComp.text;
		textComp.text = "";
	}

	public void TypeText ()
	{
		StartCoroutine (TextCoroutine ());
	}

	IEnumerator TextCoroutine ()
	{
		typing = true;

		foreach (char letter in message.ToCharArray())
		{
			textComp.text += letter;
			if (typeSound1 && typeSound2)
				SoundManager.instance.RandomizeSfx (typeSound1, typeSound2);
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}

		WrapUp ();
	}

	void WrapUp ()
	{
		textComp.CrossFadeAlpha (0, 5, true);

		Invoke ("ChangeScene", 8);
	}

	void ChangeScene ()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (1, UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}