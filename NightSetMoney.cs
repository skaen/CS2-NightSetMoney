using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using System.Text.Json.Serialization;

namespace NightSetMoney;

public class NightSetMoneyConfig : BasePluginConfig
{
    [JsonPropertyName("PluginStartTime")] public string PluginStartTime { get; set; } = "20:00:00";
    [JsonPropertyName("PluginEndTime")] public string PluginEndTime { get; set; } = "06:00:00";
    [JsonPropertyName("Money")] public string Money { get; set; } = "16000";
    [JsonPropertyName("SkipPistolRound")] public bool SkipPistolRound { get; set; } = true;
}

[MinimumApiVersion(130)]
public class NightSetMoney : BasePlugin, IPluginConfig<NightSetMoneyConfig>
{
    public override string ModuleName => "NightSetMoney";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "skaen";

    public NightSetMoneyConfig Config { get; set; } = new NightSetMoneyConfig();

    private static CCSGameRulesProxy? GameRulesProxy;
    private static readonly ConVar mp_halftime = ConVar.Find("mp_halftime")!;
    private static readonly ConVar mp_maxrounds = ConVar.Find("mp_maxrounds")!;

    public void OnConfigParsed(NightSetMoneyConfig config)
    {
        Config = config;
    }

    [ConsoleCommand("css_stime", "Show current server time")]
    [ConsoleCommand("stime", "Show current server time")]
    [CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY)]
    public void OnCommandNightVip(CCSPlayerController? controller, CommandInfo info)
    {
        var currentTime = DateTime.Now.ToString("HH:mm:ss");
        Server.PrintToConsole($"Current server time: {currentTime}");
    }

    [GameEventHandler(mode: HookMode.Post)]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (Config.SkipPistolRound && IsPistolRound()) return HookResult.Continue;

        var player = @event.Userid;
        if (!IsPlayerValid(player)) return HookResult.Continue;

        TimeSpan.TryParse(Config.PluginStartTime, out TimeSpan startTime);
        TimeSpan.TryParse(Config.PluginEndTime, out TimeSpan endTime);
        TimeSpan.TryParse(DateTime.Now.ToString("HH:mm:ss"), out TimeSpan currentTime);

        if (startTime <= currentTime || currentTime < endTime)
        {
            var moneyServices = player?.InGameMoneyServices;
            if (moneyServices == null) return HookResult.Continue;

            var maxMoney = ConVar.Find("mp_maxmoney")!.GetPrimitiveValue<int>();

            if (Config.Money.Contains("++"))
            {
                var money = int.Parse(Config.Money.Split("++")[1]);
                if (moneyServices.Account + money > maxMoney)
                    moneyServices.Account = maxMoney;
                else
                    moneyServices.Account += money;
            }
            else
            {
                var money = int.Parse(Config.Money);
                moneyServices.Account = money > maxMoney ? maxMoney : money;
            }
        }

        return HookResult.Continue;
    }

    internal static bool IsPlayerValid(CCSPlayerController? player)
    {
        if (player is null) return false;
        return player is { IsValid: true, IsBot: false, IsHLTV: false, UserId: not null };
    }

    internal static bool IsPistolRound()
    {
        if (GameRulesProxy?.IsValid is not true)
        {
            GameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();
        }

        bool halftime = mp_halftime.GetPrimitiveValue<bool>();
        int maxrounds = mp_maxrounds.GetPrimitiveValue<int>();

        return GameRulesProxy?.GameRules?.TotalRoundsPlayed == 0 ||
               (halftime && maxrounds / 2 == GameRulesProxy?.GameRules?.TotalRoundsPlayed) ||
               (GameRulesProxy?.GameRules?.GameRestart ?? false);
    }
}