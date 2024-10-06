public class Letter : MonoBehaviour
{
    public Text letterTextComponent;
    public string letterText = "";
    public bool filled;
    public int rowBelongedTo;

    public void SetLetter(string letter)
    {
        letterText = letter;
        letterTextComponent.text = letter;
    }

    public void SetColor(Color color)
    {
        letterTextComponent.color = color;
    }
}
