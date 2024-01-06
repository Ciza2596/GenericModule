using System.Collections.Generic;
using System.Linq;
using CizaAchievementModule;
using NSubstitute;
using NUnit.Framework;

public class AchievementModuleTest
{
    private const float Min = 0;

    private const string KillStatDataId = "Kill";
    private const float KillStatValue = 10;
    private const float KillStatMax = 20;

    private const string TimeStatDataId = "Time";
    private const float TimeStatMax = 1000;

    private const string MasterPlayerAchievementDataId = "MasterPlayer";

    private IAchievementModuleConfig _achievementModuleConfig;
    private AchievementModule _achievementModule;

    private List<string> _unlockedAchievementDataIds;


    [SetUp]
    public void SetUp()
    {
        _achievementModuleConfig = Substitute.For<IAchievementModuleConfig>();
        _achievementModule = new AchievementModule(_achievementModuleConfig);

        _achievementModule.OnUnlocked += OnAchievementModuleUnlocked;

        _unlockedAchievementDataIds = new List<string>();
    }


    [TestCase(true)]
    [TestCase(false)]
    public void _01_Initialize(bool isAnd)
    {
        // arrange
        InitializeConfig(isAnd);

        // act
        _achievementModule.Initialize();

        // assert
        Check_IsInitialized(true);

        Check_CurrentStat_From_AchievementModule(KillStatDataId, Min);
        Check_CurrentStat_From_AchievementModule(TimeStatDataId, Min);

        Check_IsAchievementUnlocked_From_AchievementModule(MasterPlayerAchievementDataId, false);
    }

    [Test]
    public void _02_Release()
    {
        // arrange
        _01_Initialize(true);

        // act
        _achievementModule.Release();

        // assert
        Check_IsInitialized(false);
    }

    [Test]
    public void _03_Export()
    {
        // arrange
        _01_Initialize(true);

        // act
        var exportedAchievement = _achievementModule.Export();

        // assert
        Check_ExportedAchievement(exportedAchievement);
    }

    [Test]
    public void _04_Import()
    {
        // arrange
        _01_Initialize(true);

        // act
        _achievementModule.Import(CreateExportedAchievement());

        // assert
        {
            Check_CurrentStat_From_AchievementModule(KillStatDataId, KillStatMax);
            Check_CurrentStat_From_AchievementModule(TimeStatDataId, TimeStatMax);

            Check_IsAchievementUnlocked_From_AchievementModule(MasterPlayerAchievementDataId, true);
        }
    }

    [Test]
    public void _05_SetStat()
    {
        // arrange
        _01_Initialize(true);

        // act
        _achievementModule.SetStat(KillStatDataId, KillStatValue);

        // assert
        Check_CurrentStat_From_AchievementModule(KillStatDataId, KillStatValue);
    }


    [Test]
    public void _06_AddStat()
    {
        // arrange
        _05_SetStat();

        // act
        _achievementModule.AddStat(KillStatDataId, KillStatValue);

        // assert
        Check_CurrentStat_From_AchievementModule(KillStatDataId, KillStatMax);
    }

    [Test]
    public void _07_SetStatBeTrue()
    {
        // arrange
        _01_Initialize(true);
        Check_CurrentStat_From_AchievementModule(KillStatDataId, AchievementModule.False);

        // act
        _achievementModule.SetStatBeTrue(KillStatDataId);

        // assert
        Check_CurrentStat_From_AchievementModule(KillStatDataId, AchievementModule.True);
    }

    [Test]
    public void _08_SetStatBeFalse()
    {
        // arrange
        _01_Initialize(true);
        _achievementModule.SetStatBeTrue(KillStatDataId);
        Check_CurrentStat_From_AchievementModule(KillStatDataId, AchievementModule.True);

        // act
        _achievementModule.SetStatBeFalse(KillStatDataId);

        // assert
        Check_CurrentStat_From_AchievementModule(KillStatDataId, AchievementModule.False);
    }

    [Test]
    public void _09_Achievement_Is_Unlocked_With_And()
    {
        // arrange
        _01_Initialize(true);

        Check_IsAchievementUnlocked_From_AchievementModule(MasterPlayerAchievementDataId, false);
        Check_IsAchievementUnlockedEvent(MasterPlayerAchievementDataId, false);

        // act
        _achievementModule.SetStat(KillStatDataId, KillStatMax);
        _achievementModule.SetStat(TimeStatDataId, TimeStatMax);

        // assert
        Check_IsAchievementUnlocked_From_AchievementModule(MasterPlayerAchievementDataId, true);
        Check_IsAchievementUnlockedEvent(MasterPlayerAchievementDataId, true);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void _10_Achievement_Is_Unlocked_With_Or(bool isUseKillStat)
    {
        // arrange
        _01_Initialize(false);

        Check_IsAchievementUnlocked_From_AchievementModule(MasterPlayerAchievementDataId, false);
        Check_IsAchievementUnlockedEvent(MasterPlayerAchievementDataId, false);

        // act
        if (isUseKillStat)
            _achievementModule.SetStat(KillStatDataId, KillStatMax);
        else
            _achievementModule.SetStat(TimeStatDataId, TimeStatMax);

        // assert
        Check_IsAchievementUnlocked_From_AchievementModule(MasterPlayerAchievementDataId, true);
        Check_IsAchievementUnlockedEvent(MasterPlayerAchievementDataId, true);
    }

    private void Check_IsInitialized(bool expectedValue) =>
        Assert.AreEqual(expectedValue, _achievementModule.IsInitialized, $"IsInitialized should be {expectedValue}, not {_achievementModule.IsInitialized}.");

    private void Check_CurrentStat_From_AchievementModule(string stataDataId, float expectedCurrent)
    {
        Assert.IsTrue(_achievementModule.TryGetStatReadModel(stataDataId, out var statReadModel), $"StatReadModel: {stataDataId} should be found.");
        Assert.AreEqual(expectedCurrent, statReadModel.Current, $"ExportedStat.Current should be {expectedCurrent}, not {statReadModel.Current}.");
    }

    private void Check_IsAchievementUnlocked_From_AchievementModule(string achievementDataId, bool expectedIsUnlocked)
    {
        Assert.IsTrue(_achievementModule.TryGetIsAchievementUnlocked(achievementDataId, out var isUnlocked), $"IsUnlocked: {achievementDataId} should be found.");
        Assert.AreEqual(expectedIsUnlocked, isUnlocked, $"IsUnlocked should be {expectedIsUnlocked}, not {isUnlocked}.");
    }

    private void Check_IsAchievementUnlockedEvent(string achievementDataId, bool expectedIsUnlocked)
    {
        var result = _unlockedAchievementDataIds.Contains(achievementDataId);
        var text = expectedIsUnlocked ? string.Empty : "not";
        Assert.AreEqual(expectedIsUnlocked, result, $"UnlockedAchievementDataIds should {text} contain {achievementDataId}.");
    }

    private void Check_ExportedAchievement(IExportedAchievement exportedAchievement)
    {
        foreach (var exportedStat in exportedAchievement.ExportedStatMapByStatDataId.Values.ToArray())
        {
            Assert.IsTrue(_achievementModule.TryGetStatReadModel(exportedStat.DataId, out var statReadModel), $"StatReadModel: {exportedStat.DataId} should be found.");
            Assert.AreEqual(statReadModel.Current, exportedStat.Current, $"ExportedStat.Current should be {statReadModel.Current}, not {exportedStat.Current}.");
        }

        foreach (var pair in exportedAchievement.IsUnlockedMapByAchievementDataId)
        {
            Assert.IsTrue(_achievementModule.TryGetIsAchievementUnlocked(pair.Key, out var isUnlocked), $"IsUnlocked: {pair.Key} should be found.");
            Assert.AreEqual(isUnlocked, pair.Value, $"IsUnlocked should be {isUnlocked}, not {pair.Value}.");
        }
    }

    private IExportedAchievement CreateExportedAchievement()
    {
        var exportedStatMapByDataId = new Dictionary<string, ExportedStat>();
        exportedStatMapByDataId.Add(KillStatDataId, new ExportedStat(KillStatDataId, KillStatMax));
        exportedStatMapByDataId.Add(TimeStatDataId, new ExportedStat(TimeStatDataId, TimeStatMax));

        var isUnlockedMapByAchievementDataId = new Dictionary<string, bool>();
        isUnlockedMapByAchievementDataId.Add(MasterPlayerAchievementDataId, true);

        return new ExportedAchievement(exportedStatMapByDataId, isUnlockedMapByAchievementDataId);
    }

    private void InitializeConfig(bool isAnd)
    {
        InitializeKillAndTimeStatInfoToConfig();

        if (isAnd)
            InitializeMasterPlayerAchievementInfoToConfigWith_And();
        else
            InitializeMasterPlayerAchievementInfoToConfigWith_Or();
    }

    private void InitializeKillAndTimeStatInfoToConfig()
    {
        _achievementModuleConfig.StatType.Returns(typeof(Stat));

        var statInfos = new List<IStatInfo>();

        statInfos.Add(CreateStatInfo(KillStatDataId, Min, KillStatMax));
        statInfos.Add(CreateStatInfo(TimeStatDataId, Min, TimeStatMax));

        _achievementModuleConfig.StatInfos.Returns(statInfos.ToArray());
    }

    private void InitializeMasterPlayerAchievementInfoToConfigWith_And()
    {
        var ruleInfo = CreateRuleInfo(true, new[]
        {
            CreateConditionInfo(true, KillStatDataId, KillStatMax),
            CreateConditionInfo(true, TimeStatDataId, TimeStatMax)
        });

        var achievementInfo = CreateAchievementInfo(MasterPlayerAchievementDataId, new[] { ruleInfo });

        _achievementModuleConfig.AchievementInfos.Returns(new[] { achievementInfo });
    }

    private void InitializeMasterPlayerAchievementInfoToConfigWith_Or()
    {
        var ruleInfo1 = CreateRuleInfo(true, new[] { CreateConditionInfo(true, KillStatDataId, KillStatMax) });
        var ruleInfo2 = CreateRuleInfo(true, new[] { CreateConditionInfo(true, TimeStatDataId, TimeStatMax) });

        var achievementInfo = CreateAchievementInfo(MasterPlayerAchievementDataId, new[] { ruleInfo1, ruleInfo2 });

        _achievementModuleConfig.AchievementInfos.Returns(new[] { achievementInfo });
    }

    private IStatInfo CreateStatInfo(string statDataId, float min, float max)
    {
        var statInfo = Substitute.For<IStatInfo>();
        statInfo.DataId.Returns(statDataId);
        statInfo.Min.Returns(min);
        statInfo.Max.Returns(max);
        return statInfo;
    }


    private IAchievementInfo CreateAchievementInfo(string dataId, IRuleInfo[] ruleInfos)
    {
        var achievementInfo = Substitute.For<IAchievementInfo>();
        achievementInfo.DataId.Returns(dataId);
        achievementInfo.RuleInfos.Returns(ruleInfos);
        return achievementInfo;
    }

    private IRuleInfo CreateRuleInfo(bool isEnable, IConditionInfo[] conditionInfos)
    {
        var ruleInfo = Substitute.For<IRuleInfo>();
        ruleInfo.IsEnable.Returns(isEnable);
        ruleInfo.ConditionInfos.Returns(conditionInfos);
        return ruleInfo;
    }

    private IConditionInfo CreateConditionInfo(bool isEnable, string statDataId, float value)
    {
        var conditionInfo = Substitute.For<IConditionInfo>();
        conditionInfo.IsEnable.Returns(isEnable);
        conditionInfo.StatDataId.Returns(statDataId);
        conditionInfo.Value.Returns(value);
        return conditionInfo;
    }

    private void OnAchievementModuleUnlocked(string achievementDataId) =>
        _unlockedAchievementDataIds.Add(achievementDataId);
}