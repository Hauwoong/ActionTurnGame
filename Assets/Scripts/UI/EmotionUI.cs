using TMPro;
using UnityEngine;

public class EmotionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text emotionText;
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
        runtime.LogDispatcher.Register<EmotionStackLog>(OnEmotionStack);
        runtime.LogDispatcher.Register<EmotionLevelUpLog>(OnEmotionLevelUp);
        Refresh();
    }

    private void Unbind()
    {
        if (_runtime == null) return;
        _runtime.LogDispatcher.Unregister<EmotionStackLog>(OnEmotionStack);
        _runtime.LogDispatcher.Unregister<EmotionLevelUpLog>(OnEmotionLevelUp);
        _runtime = null;
    }

    private void OnEmotionStack(EmotionStackLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnEmotionLevelUp(EmotionLevelUpLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void Refresh()
    {
        if (_runtime == null) return;
        var cr = _runtime.GetCharacterRuntime(characterId);
        emotionText.text = $"Emotion Lv.{cr.EmotionLevel} ({cr.EmotionStack}/{cr.MaxEmotionStack})";
    }
}
