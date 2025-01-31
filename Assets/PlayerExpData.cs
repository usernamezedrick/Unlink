[System.Serializable]
public class PlayerExpData
{
    public int experience;
    public int expCap;
    public int level;

    public PlayerExpData(int exp, int cap, int lvl)
    {
        experience = exp;
        expCap = cap;
        level = lvl;
    }
}
