
using UnityEngine;
using TMPro;
namespace PolandTrackerMod
{
    public static class board
    {
        public static TextMeshPro COCText;
        public static TextMeshPro CodeOfConductText;


        public static void SetCOCText(string text)
        {
            COCText.text = text;
        }
        public static void Init()
        {
            
            board.CodeOfConductText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>();
            board.COCText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>();
            CodeOfConductText.text = "TRIAGE TRACKER";
            COCText.margin = new Vector4(0, -9, 0, 0);

            COCText.richText = true;
            COCText.lineSpacing = -3;
        }
        public static void SetLoading()
        {

            var loadingTxt = @"
            
            
            
            





            <size=200%><b><color=#ffd000>ładowanie</color></b></size>
            ";
            COCText.text = parsing.addtext("", loadingTxt, "");
        }

    }
}