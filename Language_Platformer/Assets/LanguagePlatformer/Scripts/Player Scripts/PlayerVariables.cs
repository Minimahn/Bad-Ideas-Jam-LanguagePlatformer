using UnityEngine;

public class PlayerVariables : MonoBehaviour
{
    public static PlayerVariables instance {get; private set;}

    private int hearts = 3;
    private int mana = 100;
    private float moveSpeed = 5f;
    private float fallSpeed = 5f;
    private float jumpPower = 5f;
    //BOOL STATES
    private bool invulnerable = false;
    // ... etc ... 

    public void adjustHearts(int newHearts)
    {
        hearts += newHearts;
    }
    public void adjustMana(int newMana)
    {
        mana += newMana;
    }
    public void adjustMoveSpeed(float newMS)
    {
        moveSpeed += newMS;
    }
    public void adjustFallSpeed(float newFS)
    {
        fallSpeed += newFS;
    }
    public void adjustJumpPower(float newJP)
    {
        jumpPower += newJP;
    }
    public int getHearts()
    {
        return hearts;
    }
    public int getMana()
    {
        return mana;
    }
    public float getMoveSpeed()
    {
        return moveSpeed;
    }
    public float getFallSpeed()
    {
        return fallSpeed;
    }
    public float getJumpPower()
    {
        return jumpPower;
    }
    
}
