using UnityEngine.UI;
using UnityEngine;

public class UIUpdate
{

    public static void UpdateNumber(ref int Variable, int ModifyAmount,string BaseText, ref Text UIText)
    {
        Variable += ModifyAmount;
        UIText.text = BaseText + Variable;
    }
    public static void UpdateNumber(ref float Variable, float ModifyAmount, string BaseText, ref Text UIText, bool VisualRoundNumber = false)
    {
        Variable += ModifyAmount;
        if (VisualRoundNumber)
        {
            UIText.text = BaseText + Mathf.RoundToInt(ModifyAmount);
            return;
        }
        UIText.text = BaseText + Variable;
    }
}
