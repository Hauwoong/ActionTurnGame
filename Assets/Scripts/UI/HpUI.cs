using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private int characterId;

    private BattleRuntime _runtime;

    private void OnEnable()
    {
        battleManager.OnBattleCreated += Bind;
        battleManager.OnBattleEnded += Unbind;
        if (battleManager.Runtime != null)
            Bind(battleManager.Runtime);
    }

    private void OnDisable()
    {
        battleManager.OnBattleCreated -= Bind;
        battleManager.OnBattleEnded -= Unbind;
        Unbind();
    }

    private void Bind(BattleRuntime runtime)
    {
        Unbind();
        _runtime = runtime;
        runtime.LogDispatcher.Register<DamageLog>(OnDamage);
        runtime.LogDispatcher.Register<StatusDamageLog>(OnStatusDamage);
        runtime.LogDispatcher.Register<ChangeMaxHpLog>(OnMaxHpChanged);
        runtime.LogDispatcher.Register<DeathLog>(OnDeath);
        Refresh();
    }

    private void Unbind()
    {
        if (_runtime == null) return;
        _runtime.LogDispatcher.Unregister<DamageLog>(OnDamage);
        _runtime.LogDispatcher.Unregister<StatusDamageLog>(OnStatusDamage);
        _runtime.LogDispatcher.Unregister<ChangeMaxHpLog>(OnMaxHpChanged);
        _runtime.LogDispatcher.Unregister<DeathLog>(OnDeath);
        _runtime = null;
    }

    private void OnDamage(DamageLog log)
    {
        if (log.TargetId == characterId) Refresh();
    }

    private void OnStatusDamage(StatusDamageLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnMaxHpChanged(ChangeMaxHpLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnDeath(DeathLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void Refresh()
    {
        if (_runtime == null) return;
        var cr = _runtime.GetCharacterRuntime(characterId);

        if (hpText != null)
            hpText.text = $"HP: {cr.CurrentHp}/{cr.MaxHp}";

        if (hpSlider != null)
        {
            hpSlider.maxValue = cr.MaxHp;
            hpSlider.value = cr.CurrentHp;
        }
    }
}
