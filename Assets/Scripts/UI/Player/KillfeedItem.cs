using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * The kill feed UI item
 */
public class KillfeedItem : MonoBehaviour {

	//Text to appear on the UI item
	[SerializeField]
	Text text;

	/** Set the text to appear on the UI item **/
	public void Setup(string player,string source){
		text.text = "<b>" + source + "</b> killed <i>" + player + "</i>";
	}
}
