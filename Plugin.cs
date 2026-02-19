using System;
using BepInEx;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Collections;


namespace PolandTrackerMod
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
		async void Start()
		{
			//StartCoroutine(CoRutinneStart());
			//LocalButton.StartButtons();
			await Task.Delay(3000);
			board.Init();
			board.SetLoading();
			await Task.Delay(2000);
			_ = Task.Run(() =>
			{
				initialized = true;
				_ = parsing.EventSource();
			});
		}

		public static bool initialized = false;
		/*IEnumerator CoRutinneStart()
		{
			yield return new WaitForSeconds(5f);
			board.Init();
			board.SetLoading();

			yield return new WaitForSeconds(2f);
			Task.Run(() => parsing.EventSource());
		}*/

		void Update()
		{
			lock (parsing._errorQueue)
			{
				while (parsing._errorQueue.Count > 0)
				{
					string err = parsing._errorQueue.Dequeue();
					board.SetCOCText($"<color=#FF0000>{err}</color>");
				}
			}
			lock (parsing._queue)
			{
				while (parsing._queue.Count > 0)
				{
					string data = parsing._queue.Dequeue();
					board.SetCOCText(parsing.JsonToBoards(data));
				}
			}
		}
		async void Awake()
		{	
			await Task.Delay(2000);
			if (initialized) board.Init();
		}



		void OnGUI()
		{

			if (GUI.Button(new Rect (Screen.width - 100,Screen.height - 50,100,50), "PolandTrackerMod"))
			{
				board.COCText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>();
				board.CodeOfConductText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>();
				board.Init();
				_ = Task.Run(() => parsing.EventSource());
			}
		}
	}
}
