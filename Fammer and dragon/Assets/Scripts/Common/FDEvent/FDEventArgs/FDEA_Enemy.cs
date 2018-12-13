using System;

public abstract class FDEA_Enemy
{
    public string EnemyName;
    public int EnemyLevel;
}

public class FDEA_EnemyBorn : FDEA_Enemy
{
    public DateTime BronTime;
}

public class FDEA_EnemyDie : FDEA_Enemy
{
    public DateTime DeadTime;
}