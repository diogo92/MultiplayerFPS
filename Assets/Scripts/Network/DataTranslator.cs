using System;
using UnityEngine;

public class DataTranslator : MonoBehaviour {
	private static string KILLS_TAG = "[KILLS]";
	private static string DEATHS_TAG = "[DEATHS]";

	public static int DataToKills(string data){
		return int.Parse(DataToValue (data, KILLS_TAG));
	}

	public static int DataToDeaths(string data){
		return int.Parse(DataToValue (data, DEATHS_TAG));
	}

	private static string DataToValue(string data,string tag){
		string[] pieces = data.Split ('/');
		foreach (string piece in pieces) {
			if (piece.StartsWith (tag)) {
				return piece.Substring (tag.Length);
			}
		}
		return "";
	}

	public static string ValuesToData(int kills, int deaths){
		return KILLS_TAG + kills + "/" + DEATHS_TAG + deaths;
	}
}
