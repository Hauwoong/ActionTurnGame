using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaggerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text staggerText;
    [SerializeField] private Slider staggerSlider;
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
        runtime.LogDispatcher.Register<StaggerLog>(OnStagger);
        runtime.LogDispatcher.Register<StaggeredLog>(OnStaggered);
        runtime.LogDispatcher.Register<StaggerExitLog>(OnStaggerExit);
        runtime.LogDispatcher.Register<ChangeMaxStaggerLog>(OnMaxStaggerChanged);
        Refresh();
    }

    private void Unbind()
    {
        if (_runtime == null) return;
        _runtime.LogDispatcher.Unregister<StaggerLog>(OnStagger);
        _runtime.LogDispatcher.Unregister<StaggeredLog>(OnStaggered);
        _runtime.LogDispatcher.Unregister<StaggerExitLog>(OnStaggerExit);
        _runtime.LogDispatcher.Unregister<ChangeMaxStaggerLog>(OnMaxStaggerChanged);
        _runtime = null;
    }

    private void OnStagger(StaggerLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnStaggered(StaggeredLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnStaggerExit(StaggerExitLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnMaxStaggerChanged(ChangeMaxStaggerLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void Refresh()
    {
        if (_runtime == null) return;
        var cr = _runtime.GetCharacterRuntime(characterId);

        if (staggerText != null)
            staggerText.text = $"Stagger: {cr.CurrentStagger}/{cr.MaxStagger}";

        if (staggerSlider != null)
        {
            staggerSlider.maxValue = cr.MaxStagger;
            staggerSlider.value = cr.CurrentStagger;
        }
    }
}
