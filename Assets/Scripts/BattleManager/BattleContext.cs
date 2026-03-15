
public class BattleContext
{
    public Character currentActor;
    public Character target;
    // 생성자 추가
    public BattleContext(Character currentActor, Character target)
    {
        this.currentActor = currentActor;
        this.target = target;
    }
}

