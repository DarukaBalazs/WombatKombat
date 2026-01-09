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
    public Wombat wombat1 = Wombat.Base;
    public Wombat wombat2 = Wombat.Base;

    [Header("Character Datas")]
    public CharacterData Base;
    public CharacterData Dagi;
    public CharacterData Mary;

    private Wombat Cycle(Wombat w, int dir)
    {
        int count = Enum.GetValues(typeof(Wombat)).Length;
        return (Wombat)(((int)w + dir + count) % count);
    }

    public void pluss1() => wombat1 = Cycle(wombat1, 1);
    public void minus1() => wombat1 = Cycle(wombat1, -1);
    public void pluss2() => wombat2 = Cycle(wombat2, 1);
    public void minus2() => wombat2 = Cycle(wombat2, -1);

    private CharacterData GetData(Wombat w)
    {
        return w switch
        {
            Wombat.Base => Base,
            Wombat.Dagi => Dagi,
            _ => Mary
        };
    }

    public void StartGame()
    {
        CharacterSelectionManager.Instance.SetCharacters(
            GetData(wombat2),
            GetData(wombat1)
        );
    }
}
