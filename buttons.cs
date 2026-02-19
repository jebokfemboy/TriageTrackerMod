using TMPro;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;



namespace PolandTrackerMod
{
    public class LocalButton
    {


        public static void StartButtons()
        {
            CreateButton(0, "refresh", () =>
            {
                Debug.Log("refresh");
            });

        }

        private static void CreateButton(float horizontalPosition, string text, Action onButtonPressed = null)
        {
            GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Material pMaterial = null;

            Material unpMaterial = null;
            foreach (GorillaPressableButton obj in UnityEngine.Object.FindObjectsByType<CosmeticCategoryButton>(UnityEngine.FindObjectsSortMode.None))
            {
                if (obj.pressedMaterial != null)
                {
                    pMaterial = obj.pressedMaterial;
                    unpMaterial = obj.unpressedMaterial;
                    break;
                }
            }
            buttonObject.transform.parent = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.FindChildRecursive("code of conduct");
            buttonObject.transform.localPosition = new Vector3(horizontalPosition, 0.52f, 0.13f);
            buttonObject.transform.localRotation = Quaternion.Euler(353.5f, 0f, 0f);
            buttonObject.transform.localScale = new Vector3(0.1427168f, 0.1427168f, 0.1f);
            buttonObject.GetComponent<Renderer>().material = Resources.Load<Material>("plastik");
            buttonObject.GetComponent<Collider>().isTrigger = true;
            buttonObject.SetLayer(UnityLayer.GorillaInteractable);

            GameObject textObject = new GameObject();
            textObject.transform.parent = buttonObject.transform;
            textObject.transform.localPosition = Vector3.forward * 0.525f;
            textObject.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
            textObject.transform.localScale = Vector3.one;

            TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
            textMeshPro.font = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").GetComponentInChildren<GorillaComputerTerminal>()?.myScreenText?.font;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.characterSpacing = -10f;
            textMeshPro.overflowMode = TextOverflowModes.Overflow;
            textMeshPro.fontSize = 3f;
            textMeshPro.color = new Color(0.1960784f, 0.1960784f, 0.1960784f);
            textMeshPro.text = text;

            GorillaPressableButton pressableButton = buttonObject.AddComponent<GorillaPressableButton>();
            pressableButton.buttonRenderer = buttonObject.GetComponent<MeshRenderer>();
            pressableButton.unpressedMaterial = unpMaterial;// ?? Resources.Load<Material>("plastik"); ;
            pressableButton.pressedMaterial = pMaterial;// ?? Resources.Load<Material>("pressed");
            pressableButton.buttonRenderer.material = unpMaterial;
            UnityEvent onPressEvent = new UnityEvent();
            /*onPressEvent.AddListener(new UnityAction(() =>
            {
                pressableButton.StartCoroutine(ButtonColourUpdate(pressableButton));
            }));*/
            onPressEvent.AddListener(new UnityAction(onButtonPressed));
            pressableButton.onPressButton = onPressEvent;
        }

    }
}