using System;
using UnityEngine;

public enum Wombat
{
    Base,
    Dagi,
    Mary
}

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Selection")]
    public Wombat wombat1 = Wombat.Base;
    public Wombat wombat2 = Wombat.Base;

    [Header("Character Datas")]
    public CharacterData Base;
    public CharacterData Dagi;
    public CharacterData Mary;

    [Header("Visuals")]
    public WombatSpriteManager SpriteManager;

    private Wombat Cycle(Wombat current, int dir)
    {
        int count = Enum.GetValues(typeof(Wombat)).Length;
        return (Wombat)(((int)current + dir + count) % count);
    }

    public void pluss1()
    {
        wombat1 = Cycle(wombat1, 1);
        SpriteManager.ChangeSprite(2, GetWombatType(wombat1));
    }

    public void minus1()
    {
        wombat1 = Cycle(wombat1, -1);
        SpriteManager.ChangeSprite(2, GetWombatType(wombat1));
    }

    public void pluss2()
    {
        wombat2 = Cycle(wombat2, 1);
        SpriteManager.ChangeSprite(1, GetWombatType(wombat2));
    }

    public void minus2()
    {
        wombat2 = Cycle(wombat2, -1);
        SpriteManager.ChangeSprite(1, GetWombatType(wombat2));
    }

    private CharacterData GetData(Wombat w)
    {
        return w switch
        {
            Wombat.Base => Base,
            Wombat.Dagi => Dagi,
            _ => Mary
        };
    }

    private int GetWombatType(Wombat w)
    {
        return w switch
        {
            Wombat.Base => 0,
            Wombat.Dagi => 1,
            _ => 2
        };
    }

    // GOMB → START
    public void StartWombat()
    {
        CharacterSelectionManager.Instance.SetCharacters(
            GetData(wombat1),
            GetData(wombat2)
        );
    }
}
