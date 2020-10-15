using UnityEngine;
/// <summary>
/// Create your validator class and inherit TMPro.TMP_InputValidator 
/// Note that this is a ScriptableObject, so you'll have to create an instance of it via the Assets -> Create -> Input Field Validator 
/// </summary>
[CreateAssetMenu(fileName = "Ip Validator", menuName = "Input Field Validator")]
public class IpValidator : TMPro.TMP_InputValidator
{
    /// <summary>
    /// Override Validate method to implement your own validation
    /// </summary>
    /// <param name="text">This is a reference pointer to the actual text in the input field; changes made to this text argument will also result in changes made to text shown in the input field</param>
    /// <param name="pos">This is a reference pointer to the input field's text insertion index position (your blinking caret cursor); changing this value will also change the index of the input field's insertion position</param>
    /// <param name="ch">This is the character being typed into the input field</param>
    /// <returns>Return the character you'd allow into </returns>
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if (!(char.IsNumber(ch) || ch == '.') || text.Length > 14)
            return '\0';

        if (ch == '.')
        {
            if (pos != 0 && GetCount('.', text) < 4)
                text = text.Insert(pos, ch.ToString());
            else
                return '\0';
        }
        else
        {
            int l = text.Length;
            string[] a = text.Split('.');
            for (int i = a.Length - 1; i >= 0; i--)
            {
                l -= a[i].Length;
                if (pos >= l)
                {
                    if (a[i].Length > 2) 
                        return '\0';
                    text = text.Insert(pos, ch.ToString());
                    break;
                }
            }
        }


        pos++;
        return ch;
    }

    private int GetCount(char it, string s)
    {
        int count = 0;
        foreach(char c in s)
        {
            if (c == it) count++;
        }

        return count;
    }
}