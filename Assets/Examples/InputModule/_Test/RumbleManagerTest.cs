using CizaInputModule;
using NSubstitute;
using NUnit.Framework;

public class RumbleManagerTest
{
	public const string DataId = "Default";

	public const float Duration = 2;

	public const float LowFrequency  = 1.5f;
	public const float HighFrequency = 1;

	public const int PlayerCount = 2;

	public const int Index0 = 0;
	public const int Index1 = 1;

	private IRumbleManagerConfig _rumbleManagerConfig;
	private IRumbleInputs        _rumbleInputs;

	private RumbleManager _rumbleManager;

	[SetUp]
	public void Setup()
	{
		_rumbleManagerConfig = Substitute.For<IRumbleManagerConfig>();
		_rumbleInputs        = Substitute.For<IRumbleInputs>();

		_rumbleManager = new RumbleManager(_rumbleManagerConfig, _rumbleInputs);
	}

	[Test]
	public void _01_Rumble()
	{
		// arrange
		_rumbleManagerConfig.TryGetRumbleInfo(DataId, out var rumbleInfo).Returns(x =>
		{
			x[1] = CreateRumbleInfo();
			return true;
		});

		_rumbleInputs.PlayerCount.Returns(PlayerCount);

		// act
		_rumbleManager.Rumble(Index0, DataId);

		// assert
		_rumbleInputs.Received().SetMotorSpeeds(Index0, LowFrequency, HighFrequency);
	}

	[Test]
	public void _02_RumbleAll()
	{
		// arrange
		_rumbleManagerConfig.TryGetRumbleInfo(DataId, out var rumbleInfo).Returns(x =>
		{
			x[1] = CreateRumbleInfo();
			return true;
		});

		_rumbleInputs.PlayerCount.Returns(PlayerCount);

		// act
		_rumbleManager.RumbleAll(DataId);

		// assert
		_rumbleInputs.Received().SetMotorSpeeds(Index0, LowFrequency, HighFrequency);
		_rumbleInputs.Received().SetMotorSpeeds(Index1, LowFrequency, HighFrequency);
	}

	[Test]
	public void _03_Stop()
	{
		// arrange
		_01_Rumble();

		// act
		_rumbleManager.Stop(Index0);

		// assert
		_rumbleInputs.Received().ResetHaptics(Index0);
	}

	[Test]
	public void _04_StopAll()
	{
		// arrange
		_02_RumbleAll();

		// act
		_rumbleManager.StopAll();

		// assert
		_rumbleInputs.Received().ResetHaptics(Index0);
		_rumbleInputs.Received().ResetHaptics(Index1);
	}

	[Test]
	public void _05_AutoStop()
	{
		// arrange
		_01_Rumble();

		// act
		_rumbleManager.Tick(Duration + 0.1f);

		// assert
		_rumbleInputs.Received().ResetHaptics(Index0);
	}

	private IRumbleInfo CreateRumbleInfo() =>
		IRumbleInfo.CreateRumbleInfo(DataId, Duration, LowFrequency, HighFrequency);
}
