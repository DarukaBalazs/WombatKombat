using UnityEngine;

public enum Wombat 
{
    Dagi,
    Base,
    Mary
}
public class CharacterSelectManager : MonoBehaviour
{
    private Wombat wombat1 = Wombat.Base;
    private Wombat wombat2 = Wombat.Base;
    public CharacterData Base;
    public CharacterData Mary;
    public CharacterData Dagi;
    public CharacterSelectorú Selector;

    public void pluss1()
    {
        wombat1++;
    }

    public void minus1()
    {
        wombat1--;
    }

    public void pluss2()
    {
        wombat2++;
    }

    public void minus2()
    {
        wombat2--;
    }
    private CharacterData Selection(Wombat wombat)
    {
        switch (wombat)
        {
            case Wombat.Base:
                return Base;
                
            case Wombat.Dagi:
                return  Dagi;
                
            default:
                return Mary;
                

        }

    }

    public void StarWombat()
    {
        Selector.player1 = Selection(wombat1);
        Selector.player2 = Selection(wombat2);
        
    }
}