using TMPro;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text energyText;
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
        runtime.LogDispatcher.Register<EnergyUseLog>(OnEnergyUse);
        runtime.LogDispatcher.Register<ChangeMaxEnergyLog>(OnMaxEnergyChanged);
        Refresh();
    }

    private void Unbind()
    {
        if (_runtime == null) return;
        _runtime.LogDispatcher.Unregister<EnergyUseLog>(OnEnergyUse);
        _runtime.LogDispatcher.Unregister<ChangeMaxEnergyLog>(OnMaxEnergyChanged);
        _runtime = null;
    }

    private void OnEnergyUse(EnergyUseLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnMaxEnergyChanged(ChangeMaxEnergyLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void Refresh()
    {
        if (_runtime == null) return;
        var cr = _runtime.GetCharacterRuntime(characterId);
        energyText.text = $"Energy: {cr.CurrentEnergy}/{cr.MaxEnergy}";
    }
}
